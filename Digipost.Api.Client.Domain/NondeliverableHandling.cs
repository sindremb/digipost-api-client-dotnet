﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digipost.Api.Client.Domain
{
    // NondeliverableHandling
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "nondeliverable-handling", Namespace = "http://api.digipost.no/schema/v6")]
    public enum NondeliverableHandling
    {

        /// <remarks/>
        SHRED,

        /// <remarks/>
        RETURN_TO_SENDER,
    }
}
