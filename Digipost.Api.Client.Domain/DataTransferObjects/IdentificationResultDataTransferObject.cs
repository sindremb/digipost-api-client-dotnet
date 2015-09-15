﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using Digipost.Api.Client.Domain.Enums;

namespace Digipost.Api.Client.Domain.DataTransferObjects
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(TypeName = "identification-result", Namespace = "http://api.digipost.no/schema/v6")]
    [XmlRoot(Namespace = "http://api.digipost.no/schema/v6", IsNullable = false)]
    public class IdentificationResultDataTransferObject
    {
        [XmlElement("result")]
        public IdentificationResultCode IdentificationResultCode { get; set; }

        [XmlElement("digipost-address", typeof (string))]
        [XmlElement("invalid-reason", typeof (InvalidReason))]
        [XmlElement("person-alias", typeof (string))]
        [XmlElement("unidentified-reason", typeof (UnidentifiedReason))]
        [XmlChoiceIdentifier("IdentificationResultType")]
        public object IdentificationValue { get;  set; }

        [XmlIgnore]
        public IdentificationResultType IdentificationResultType { get;  set; }

        internal IdentificationResultDataTransferObject() { }

        public override string ToString()
        {
            return string.Format("IdentificationResultCode: {0}, IdentificationType: {1}, IdentificationValue: {2}",
                IdentificationResultCode, IdentificationResultType, IdentificationValue);
        }
    }
}