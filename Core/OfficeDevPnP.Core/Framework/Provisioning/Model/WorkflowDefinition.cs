﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OfficeDevPnP.Core.Extensions;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Defines a Workflow Definition to provision
    /// </summary>
    public class WorkflowDefinition: IEquatable<WorkflowDefinition>
    {
        #region Private Members

        private Dictionary<String, String> _properties = new Dictionary<String, String>;

        #endregion

        #region Public Members

        /// <summary>
        /// Defines the Properties of the Workflows to provision
        /// </summary>
        public Dictionary<String, String> Properties
        {
            get { return this._properties; }
            set {  this._properties = value; }
        }

        /// <summary>
        /// Defines the FormField XML of the Workflow to provision
        /// </summary>
        public String FormField { get; set; }

        /// <summary>
        /// Defines the ID of the Workflow Definition for the current Subscription
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Defines the URL of the Workflow Association page
        /// </summary>
        public String AssociationUrl { get; set; }

        /// <summary>
        /// The Description of the Workflow
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// The Display Name of the Workflow
        /// </summary>
        public String DisplayName { get; set; }

        /// <summary>
        /// Defines the URL of the Workflow Initiation page
        /// </summary>
        public String InitiationUrl { get; set; }

        /// <summary>
        /// Defines if the Workflow requires the Association Form
        /// </summary>
        public Boolean RequiresAssociationForm { get; set; }

        /// <summary>
        /// Defines if the Workflow requires the Initiation Form
        /// </summary>
        public Boolean RequiresInitiationForm { get; set; }

        /// <summary>
        /// Defines the Scope Restriction for the Workflow
        /// </summary>
        public Boolean RestrictToScope { get; set; }

        /// <summary>
        /// Defines the Type of Scope Restriction for the Workflow
        /// </summary>
        public Boolean RestrictToType { get; set; }

        /// <summary>
        /// Defines the XAML of the Workflow to provision
        /// </summary>
        public XElement Xaml { get; set; }

        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|",
                this.Properties.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.FormField.GetHashCode(),
                this.Id.GetHashCode(),
                this.AssociationUrl.GetHashCode(),
                this.Description.GetHashCode(),
                this.DisplayName.GetHashCode(),
                this.InitiationUrl.GetHashCode(),
                this.RequiresAssociationForm.GetHashCode(),
                this.RequiresInitiationForm.GetHashCode(),
                this.RestrictToScope.GetHashCode(),
                this.RestrictToType.GetHashCode(),
                this.Xaml.ToString().GetHashCode()
            ).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is WorkflowDefinition))
            {
                return (false);
            }
            return (Equals((WorkflowDefinition)obj));
        }

        public bool Equals(WorkflowDefinition other)
        {
            return (
                this.Properties.DeepEquals(other.Properties) &&
                this.FormField == other.FormField &&
                this.Id == other.Id &&
                this.AssociationUrl == other.AssociationUrl &&
                this.Description == other.Description &&
                this.DisplayName == other.DisplayName &&
                this.InitiationUrl == other.InitiationUrl &&
                this.RequiresAssociationForm == other.RequiresAssociationForm &&
                this.RequiresInitiationForm == other.RequiresInitiationForm &&
                this.RestrictToScope == other.RestrictToScope &&
                this.RestrictToType == other.RestrictToType &&
                this.Xaml.Equals(other.Xaml)
                );
        }

        #endregion
    }
}
