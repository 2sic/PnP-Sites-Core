﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Diagnostics;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class ObjectAuditSettings : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Audit Settings"; }
        }

        public override ProvisioningTemplate ExtractObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            using (var scope = new PnPMonitoredScope("Audit Settings"))
            {
                var auditSettings = new AuditSettings();

                var site = (web.Context as ClientContext).Site;
                var siteAuditSettings = site.Audit;
                web.Context.Load(siteAuditSettings);
                web.Context.Load(site, s => s.AuditLogTrimmingRetention, s => s.TrimAuditLog);
                web.Context.ExecuteQueryRetry();

                auditSettings.AuditFlag = siteAuditSettings.AuditFlags;
                auditSettings.AuditLogTrimmingRetention = site.AuditLogTrimmingRetention;
                auditSettings.TrimAuditLog = site.TrimAuditLog;

                template.AuditSettings = auditSettings;
            }
            return template;
        }

        public override TokenParser ProvisionObjects(Web web, ProvisioningTemplate template, TokenParser parser, ProvisioningTemplateApplyingInformation applyingInformation)
        {
            using (var scope = new PnPMonitoredScope("Audit Settings"))
            {
                var site = (web.Context as ClientContext).Site;
                var siteAuditSettings = site.Audit;
                web.Context.Load(siteAuditSettings);
                web.Context.Load(site, s => s.AuditLogTrimmingRetention, s => s.TrimAuditLog);
                web.Context.ExecuteQueryRetry();

                var isDirty = false;
                if (template.AuditSettings.AuditFlag != siteAuditSettings.AuditFlags)
                {
                    site.Audit.AuditFlags = template.AuditSettings.AuditFlag;
                    site.Audit.Update();
                    isDirty = true;
                }
                if (template.AuditSettings.AuditLogTrimmingRetention != site.AuditLogTrimmingRetention)
                {
                    site.AuditLogTrimmingRetention = template.AuditSettings.AuditLogTrimmingRetention;
                    isDirty = true;
                }
                if (template.AuditSettings.TrimAuditLog != site.TrimAuditLog)
                {
                    site.TrimAuditLog = template.AuditSettings.TrimAuditLog;
                    isDirty = true;
                }
                if (isDirty)
                {
                    web.Context.ExecuteQueryRetry();
                }
            }

            return parser;
        }

        public override bool WillExtract(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            return !web.IsSubSite();
        }

        public override bool WillProvision(Web web, ProvisioningTemplate template)
        {
            return !web.IsSubSite();
        }
    }
}
