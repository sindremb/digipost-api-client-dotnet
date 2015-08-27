﻿using System.Collections.Generic;
using System.Linq;
using Digipost.Api.Client.Domain.Print;
using Digipost.Api.Client.Tests.CompareObjects;
using Digipost.Api.Client.Tests.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digipost.Api.Client.Tests.Unittest.DtoTests
{
    [TestClass]
    public class PrintReturnRecipientTests
    {
        [TestClass]
        public class ConstructorMethod : PrintReturnRecipientTests
        {
            readonly Comparator _comparator = new Comparator();

            [TestMethod]
            public void SimpleConstructor()
            {
                //Arrange
                PrintReturnRecipient printRecipient = new PrintReturnRecipient("name", DomainUtility.GetNorwegianAddress());

                //Act

                //Assert
                Assert.AreEqual("name", printRecipient.Name);

                IEnumerable<IDifference> differences;
                _comparator.AreEqual(DomainUtility.GetNorwegianAddress(), printRecipient.Address, out differences);
                
                Assert.AreEqual(0, differences.Count());
            }
        }
    }
}
