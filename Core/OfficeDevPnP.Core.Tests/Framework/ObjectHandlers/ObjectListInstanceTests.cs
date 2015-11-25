﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;

namespace OfficeDevPnP.Core.Tests.Framework.ObjectHandlers
{
    [TestClass]
    public class ObjectListInstanceTests
    {
        private string listName;
       
        [TestInitialize]
        public void Initialize()
        {
            listName = string.Format("Test_{0}", DateTime.Now.Ticks);
            
        }
        [TestCleanup]
        public void CleanUp()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                var list = ctx.Web.GetListByUrl(string.Format("lists/{0}",listName));
                if (list != null)
                {
                    list.DeleteObject();
                    ctx.ExecuteQueryRetry();
                }
            }
        }

        [TestMethod]
        public void CanProvisionObjects()
        {
            var template = new ProvisioningTemplate();
            var listInstance = new Core.Framework.Provisioning.Model.ListInstance();

            listInstance.Url = string.Format("lists/{0}", listName);
            listInstance.Title = listName;
            listInstance.TemplateType = (int) ListTemplateType.GenericList;

            Dictionary<string, string> dataValues = new Dictionary<string, string>();
            dataValues.Add("Title","Test");
            DataRow dataRow = new DataRow(dataValues);

            listInstance.DataRows.Add(dataRow);

            template.Lists.Add(listInstance);

            using (var ctx = TestCommon.CreateClientContext())
            {
                var parser = new TokenParser(ctx.Web, template);

                // Create the List
                parser = new ObjectListInstance().ProvisionObjects(ctx.Web, template, parser, new ProvisioningTemplateApplyingInformation());

                // Load DataRows
                new ObjectListInstanceDataRows().ProvisionObjects(ctx.Web, template, parser, new ProvisioningTemplateApplyingInformation());

                var list = ctx.Web.GetListByUrl(listInstance.Url);
                Assert.IsNotNull(list);
                
                var items = list.GetItems(CamlQuery.CreateAllItemsQuery());
                ctx.Load(items, itms => itms.Include(item => item["Title"]));
                ctx.ExecuteQueryRetry();

                Assert.IsTrue(items.Count == 1);
                Assert.IsTrue(items[0]["Title"].ToString() == "Test");
            }
        }

        [TestMethod]
        public void CanCreateEntities()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                // Load the base template which will be used for the comparison work
                var creationInfo = new ProvisioningTemplateCreationInformation(ctx.Web) {BaseTemplate = ctx.Web.GetBaseTemplate()};

                var template = new ProvisioningTemplate();
                template = new ObjectListInstance().ExtractObjects(ctx.Web, template, creationInfo);

                Assert.IsTrue(template.Lists.Any());
            }
        }

        [TestMethod]
        public void UpdatedListTitleShouldBeAvailableAsToken()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                var updatedListTitle = listName + "_edit";
                var mockProviderType = typeof(MockProviderForListInstanceTests);
                var configData = "{listid:" + updatedListTitle + "}+{listurl:" + updatedListTitle + "}";
                var listUrl = string.Format("lists/{0}", listName);

                var listInstance = new Core.Framework.Provisioning.Model.ListInstance();
                listInstance.Url = listUrl;
                listInstance.Title = updatedListTitle;
                listInstance.TemplateType = (int)ListTemplateType.GenericList;
                var template = new ProvisioningTemplate();
                template.Lists.Add(listInstance);
                template.Providers.Add(new Provider() { Assembly = mockProviderType.Assembly.FullName, Type = mockProviderType.FullName, Enabled = true, Configuration = configData });

                // Create a list and then let the provisioning engine change it's title
                var list = ctx.Web.Lists.Add(new ListCreationInformation() { Title = listName, TemplateType = (int)ListTemplateType.GenericList, Url = listUrl });
                list.EnsureProperty(l => l.Id);
                ctx.ExecuteQueryRetry();
                var listId = list.Id.ToString();
                ctx.Web.ApplyProvisioningTemplate(template);

                var expectedConfig = string.Format("{0}+{1}", listId, listInstance.Url).ToLower();
                Assert.AreEqual(expectedConfig, MockProviderForListInstanceTests.ConfigurationData.ToLower(), "Updated list title is not available as a token.");
            }           
        }

        class MockProviderForListInstanceTests : OfficeDevPnP.Core.Framework.Provisioning.Extensibility.IProvisioningExtensibilityProvider
        {
            public static string ConfigurationData { get; private set; }
            public void ProcessRequest(ClientContext ctx, ProvisioningTemplate template, string configurationData)
            {
                ConfigurationData = configurationData;
            }
        }
    }


}
