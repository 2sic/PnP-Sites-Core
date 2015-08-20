﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// The Audit Settings for the Provisioning Template
    /// </summary>
    public class AuditSettings : IEquatable<AuditSettings>
    {
        /// <summary>
        /// Audit Flags configured for the Site
        /// </summary>
        public Microsoft.SharePoint.Client.AuditMaskType AuditFlag { get; set; }

        /// <summary>
        /// The Audit Log Trimming Retention for Audits
        /// </summary>
        public Int32 AuditLogTrimmingRetention { get; set; }

        /// <summary>
        /// A flag to enable Audit Log Trimming
        /// </summary>
        public Boolean TrimAuditLog { get; set; }

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}",
                this.AuditFlag,
                this.AuditLogTrimmingRetention,
                this.TrimAuditLog
                ).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AuditSettings))
            {
                return (false);
            }
            return (Equals((AuditSettings)obj));
        }

        public bool Equals(AuditSettings other)
        {
            return (this.AuditFlag == other.AuditFlag  &&
                this.AuditLogTrimmingRetention == other.AuditLogTrimmingRetention &&
                this.TrimAuditLog == other.TrimAuditLog
                );
        }

        #endregion
    }
}
