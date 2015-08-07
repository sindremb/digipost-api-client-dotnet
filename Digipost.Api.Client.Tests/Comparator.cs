﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Digipost.Api.Client.Domain;
using Digipost.Api.Client.Domain.Autocomplete;
using Digipost.Api.Client.Domain.PersonDetails;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digipost.Api.Client.Tests
{
    public class Comparator
    {
        public static void LookLikeEachOther(object expected, object actual)
        {
            var typeExpected = expected != null ? expected.GetType() : null;
            var typeActual = actual != null ? actual.GetType() : null;

            Assert.AreEqual(typeExpected, typeActual, "The types of instances expected and actual are not the same.");

            var myProperties = typeExpected.GetProperties(BindingFlags.DeclaredOnly
                                | BindingFlags.Public | BindingFlags.Instance);

            foreach (var myPropertyA in myProperties)
            {
                var myPropertyB = typeActual.GetProperty(myPropertyA.Name);
                Assert.IsNotNull(myPropertyB, string.Format(@"The property {0} from instance expected 
           was not found on instance actual.", myPropertyA.Name));

                Assert.AreEqual<Type>(myPropertyA.PropertyType, myPropertyB.PropertyType,
                       string.Format(@"The type of property {0} on instance expected is different from 
                       the one on instance actual.", myPropertyA.Name));
                Console.WriteLine(myPropertyA.Name);

                var valueA = myPropertyA.GetValue(expected, null);
                var valueB = myPropertyB.GetValue(actual, null);

                if ((valueA == null || valueB == null) && valueA == valueB)
                    continue;

                if (IsList(valueA) && IsList(valueB))
                {
                    var aType = valueA.GetType();
                    if (aType == typeof(List<Listedtime>))
                    {
                        CheckList((IEnumerable<Listedtime>)valueA, (IEnumerable<Listedtime>)valueB);
                    }
                    else if (aType == typeof(List<int>))
                    {
                        CheckList((IEnumerable<int>)valueA, (IEnumerable<int>)valueB);
                    }
                    else if (aType == typeof(List<Document>))
                    {
                        CheckList((IEnumerable<Document>)valueA, (IEnumerable<Document>)valueB);
                    }
                    else if (aType == typeof(List<AutocompleteSuggestion>))
                    {
                        CheckList((IEnumerable<AutocompleteSuggestion>)valueA, (IEnumerable<AutocompleteSuggestion>)valueB);
                    }
                    else if (aType == typeof(List<PersonDetails>))
                    {
                        CheckList((IEnumerable<PersonDetails>)valueA, (IEnumerable<PersonDetails>)valueB);
                    }
                    else
                    {
                        Assert.Fail("Unkown type in list." + aType);
                    }

                    continue;
                }

                TestPrimitiveValue(valueA, valueB);
            }
        }

        private static T CastExamp1<T>(object input)
        {
            return (T)input;
        }

        public static void CheckList<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            foreach (var l1 in list1)
            {
                foreach (var l2 in list2)
                {
                    TestPrimitiveValue(l1, l2);
                }
            }
        }


        private static void TestPrimitiveValue(object valueA, object valueB)
        {
            if (valueA != null && !PrimitiveTypes.Test(valueA.GetType()))
            {
                LookLikeEachOther(valueA, valueB);
            }
            else
            {
                Assert.AreEqual(valueA, valueB,
                    string.Format(@"The value {1}  of the property {0} on instance expected is different from 
            the value {2}  on instance actual.", (valueA == null ? "null" : valueA.GetType().Name), valueA, valueB));
            }
        }

        public static bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static bool IsDictionary(object o)
        {
            if (o == null) return false;
            return o is IDictionary &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }
        static class PrimitiveTypes
        {
            private static readonly Type[] List;

            static PrimitiveTypes()
            {
                var types = new[]
                          {
                              typeof (Enum),
                              typeof (String),
                              typeof (Char),
                              typeof (Guid),

                              typeof (Boolean),
                              typeof (Byte),
                              typeof (Int16),
                              typeof (Int32),
                              typeof (Int64),
                              typeof (Single),
                              typeof (Double),
                              typeof (Decimal),

                              typeof (SByte),
                              typeof (UInt16),
                              typeof (UInt32),
                              typeof (UInt64),

                              typeof (DateTime),
                              typeof (DateTimeOffset),
                              typeof (TimeSpan),
                          };


                var nullTypes = from t in types
                                where t.IsValueType
                                select typeof(Nullable<>).MakeGenericType(t);

                List = types.Concat(nullTypes).ToArray();
            }

            public static bool Test(Type type)
            {
                if (List.Any(x => x.IsAssignableFrom(type)))
                    return true;

                var nut = Nullable.GetUnderlyingType(type);
                return nut != null && nut.IsEnum;
            }
        }
    }
}
