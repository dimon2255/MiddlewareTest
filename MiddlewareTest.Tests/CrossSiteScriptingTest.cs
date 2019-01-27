using MiddlewareTest.StringExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace MiddlewareTest.Tests
{
    public class CrossSiteScriptingTest
    {
        private char[] _dangerousChars = new char[] { '<', '>', '*', '%', '&', ';', ':', '\\', '?' };

        [Fact]
        public void Pass_When_No_Dandgerous_Chars_In_Json()
        {
            var merchant = new Merchant()
            {
                Id = 1,
                DateCreated = DateTime.Now,
                Value = "Hello",
                MposUser = new MposUser()
                {
                    Id = 2,
                    DivisionName = "Visa",
                    MposUserCredentials = new List<MposUserCredentials>()
                    {
                      new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                       new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                       new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                    },

                    Values = new string[] { "Hello", "Bye, Dimitri", "Pankov" }
                }
            };

            var json = JsonConvert.SerializeObject(merchant);

            Assert.False(json.DetectDangerousChars());
        }


        [Fact]
        public void Find_Dangerous_Chars_In_Merchant_Value()
        {
            var merchant = new Merchant()
            {
                Id = 1,
                DateCreated = DateTime.Now,
                Value = "Hell;o",
                MposUser = new MposUser()
                {
                    Id = 2,
                    DivisionName = "Visa",
                    MposUserCredentials = new List<MposUserCredentials>()
                    {
                      new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                       new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                       new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                    },

                    Values = new string[] { "Hello", "Bye, Dimitri", "Pankov" }
                }
            };

            var json = JsonConvert.SerializeObject(merchant);

            Assert.True(json.DetectDangerousChars());
        }

        [Fact]
        public void Find_Dangerous_Chars_In_MposUserCredentials_List()
        {
            var merchant = new Merchant()
            {
                Id = 1,
                DateCreated = DateTime.Now,
                Value = "Hello",
                MposUser = new MposUser()
                {
                    Id = 2,
                    DivisionName = "Visa",
                    MposUserCredentials = new List<MposUserCredentials>()
                    {
                      new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                       new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                       new MposUserCredentials()
                        {
                            Username = "Dimitr><>i",
                            Password = "baseball"
                        },
                    },

                    Values = new string[] { "Hello", "Bye, Dimitri", "Pankov" }
                }
            };

            var json = JsonConvert.SerializeObject(merchant);

            Assert.True(json.DetectDangerousChars());
        }

        [Fact]
        public void Find_Dangerous_Chars_In_DivisionName()
        {
            var merchant = new Merchant()
            {
                Id = 1,
                DateCreated = DateTime.Now,
                Value = "Hello",
                MposUser = new MposUser()
                {
                    Id = 2,
                    DivisionName = "Visa*%O^><",
                    MposUserCredentials = new List<MposUserCredentials>()
                    {
                      new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                       new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                       new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                    },

                    Values = new string[] { "Hello", "Bye, Dimitri", "Pankov" }
                }
            };

            var json = JsonConvert.SerializeObject(merchant);

            Assert.True(json.DetectDangerousChars());
        }

        [Fact]
        public void Skip_When_Dandgerous_Chars_In_Password()
        {
            var merchant = new Merchant()
            {
                Id = 1,
                DateCreated = DateTime.Now,
                Value = "Hello",
                MposUser = new MposUser()
                {
                    Id = 2,
                    DivisionName = "Visa",
                    MposUserCredentials = new List<MposUserCredentials>()
                    {
                      new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                       new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseb><>><*(&&^$(^all"
                        },
                       new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                    },

                    Values = new string[] { "Hello", "Bye, Dimitri", "Pankov" }
                }
            };

            var json = JsonConvert.SerializeObject(merchant);

            Assert.False(json.DetectDangerousChars());
        }

        [Fact]
        public void Find_Dangerous_Chars_In_JsonArray()
        {
            var merchant = new Merchant()
            {
                Id = 1,
                DateCreated = DateTime.Now,
                Value = "Hello",
                MposUser = new MposUser()
                {
                    Id = 2,
                    DivisionName = "Visa",
                    MposUserCredentials = new List<MposUserCredentials>()
                    {
                      new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                       new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                       new MposUserCredentials()
                        {
                            Username = "Dimitri",
                            Password = "baseball"
                        },
                    },
                    Values = new string[] { "Hel><><>lo", "By&(^#%@e, Dimitri", "Pankov" }
                }
            };

            var json = JsonConvert.SerializeObject(merchant);

            Assert.True(json.DetectDangerousChars());
        }

        public class Merchant
        {
            public int Id { get; set; }
            public DateTime DateCreated { get; set; }
            public string Value { get; set; }
            public MposUser MposUser { get; set; }
        }

        public class MposUser
        {
            public string DivisionName { get; set; }
            public int Id { get; set; }
            public List<MposUserCredentials> MposUserCredentials { get; set; }
            public string[] Values { get; set; }
        }

        public class MposUserCredentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}