﻿using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Database.Impl.Generators;
using Raven.Tests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Raven.Tests.Json
{
    public class JsonCodeGeneratorTests : RavenTest
    {
        public class WithStrings
        {
            public string Title { get; set; }

            public string Category { get; set; }
        }

        public class WithIntsAndDateTimes
        {
            public int Integer { get; set; }
            public long Long { get; set; }
            public DateTime Date { get; set; }
        }

        public class With2Uris
        {
            public string UriButString { get; set; }
            public Uri Uri { get; set; }
        }

        public class WithTheRest
        {
            public TimeSpan Time { get; set; }
            public Guid Guid { get; set; }
            
            public DateTime Date { get; set; }
            public DateTimeOffset DateOffset { get; set; }

            public byte[] Bytes { get; set; }
            public bool Boolean { get; set; }

            public string Null { get; set; }
        }

        public class WithInnerObject
        {
            public WithIntsAndDateTimes First { get; set; }
            public WithStrings Second { get; set; }
        }

        public class WithArrayOfBasics
        {
            public IList<int> Ints { get; set; }
            public IList<string> Strings { get; set; }
        }

        public class WithArrayOfObjects
        {
            public IList<WithStrings> Objects { get; set; }
        }

        public class WithArrayOfFloats
        {
            public IList<float> Floats { get; set; }
        }


        [Fact]
        public void JsonCodeGenerator_SimpleObjectWithStrings()
        {
            var root = new WithStrings() 
            {
                Title = "test",
                Category = "category" 
            };

            var generator = new JsonCodeGenerator("csharp");
            var classTypes = generator.GenerateClassTypesFromObject("Root", JsonExtensions.ToJObject(root))
                                      .ToLookup(x => x.Name);

            Assert.Equal(1, classTypes.Count());

            var clazz = classTypes["Root"].Single() as JsonCodeGenerator.ClassType;
            Assert.NotNull(clazz);

            Assert.Equal("string", clazz.Properties["Title"].Name);
            Assert.False(clazz.Properties["Title"].IsArray);
            Assert.True(clazz.Properties["Title"].IsPrimitive);

            Assert.Equal("string", clazz.Properties["Category"].Name);
            Assert.False(clazz.Properties["Category"].IsArray);
            Assert.True(clazz.Properties["Category"].IsPrimitive);
        }

        [Fact]
        public void JsonCodeGenerator_SimpleObjectWithNumericsAndDateTime()
        {
            var root = new WithIntsAndDateTimes() 
            { 
                Integer = int.MaxValue, 
                Long = long.MaxValue,                 
                Date = DateTime.Now 
            };

            var generator = new JsonCodeGenerator("csharp");
            var classTypes = generator.GenerateClassTypesFromObject("Root", JsonExtensions.ToJObject(root))
                                      .ToLookup(x => x.Name);

            Assert.Equal(1, classTypes.Count());

            var clazz = classTypes["Root"].Single() as JsonCodeGenerator.ClassType;
            Assert.NotNull(clazz);

            Assert.Equal("int", clazz.Properties["Integer"].Name);
            Assert.False(clazz.Properties["Integer"].IsArray);
            Assert.True(clazz.Properties["Integer"].IsPrimitive);

            Assert.Equal("long", clazz.Properties["Long"].Name);
            Assert.False(clazz.Properties["Long"].IsArray);
            Assert.True(clazz.Properties["Long"].IsPrimitive);

            Assert.Equal("DateTimeOffset", clazz.Properties["Date"].Name);
            Assert.False(clazz.Properties["Date"].IsArray);
            Assert.True(clazz.Properties["Date"].IsPrimitive);
        }

        [Fact]
        public void JsonCodeGenerator_ContentResolutionForNumerics()
        {
            // Result will be Int,Int,DateTime
            var root = new WithIntsAndDateTimes()
            {
                Integer = 1,
                Long = 2,
                Date = DateTime.Now
            };

            var generator = new JsonCodeGenerator("csharp");
            var classTypes = generator.GenerateClassTypesFromObject("Root", JsonExtensions.ToJObject(root))
                                      .ToLookup(x => x.Name);

            Assert.Equal(1, classTypes.Count());


            var clazz = classTypes["Root"].Single() as JsonCodeGenerator.ClassType;
            Assert.NotNull(clazz);

            Assert.Equal("int", clazz.Properties["Integer"].Name);
            Assert.False(clazz.Properties["Integer"].IsArray);
            Assert.True(clazz.Properties["Integer"].IsPrimitive);

            Assert.Equal("int", clazz.Properties["Long"].Name);
            Assert.False(clazz.Properties["Long"].IsArray);
            Assert.True(clazz.Properties["Long"].IsPrimitive);

            Assert.Equal("DateTimeOffset", clazz.Properties["Date"].Name);
            Assert.False(clazz.Properties["Date"].IsArray);
            Assert.True(clazz.Properties["Date"].IsPrimitive);
        }

        [Fact]
        public void JsonCodeGenerator_ContentResolutionForUris()
        {
            // Result will be Int,Int,DateTime
            var root = new With2Uris()
            {
                UriButString = "http://localhost",
                Uri = new Uri("http://localhost"),
            };

            var generator = new JsonCodeGenerator("csharp");
            var classTypes = generator.GenerateClassTypesFromObject("Root", JsonExtensions.ToJObject(root))
                                      .ToLookup(x => x.Name);

            Assert.Equal(1, classTypes.Count());


            var clazz = classTypes["Root"].Single() as JsonCodeGenerator.ClassType;
            Assert.NotNull(clazz);

            Assert.Equal("Uri", clazz.Properties["UriButString"].Name);
            Assert.False(clazz.Properties["UriButString"].IsArray);
            Assert.True(clazz.Properties["UriButString"].IsPrimitive);

            Assert.Equal("Uri", clazz.Properties["Uri"].Name);
            Assert.False(clazz.Properties["Uri"].IsArray);
            Assert.True(clazz.Properties["Uri"].IsPrimitive);
        }

        [Fact]
        public void JsonCodeGenerator_ContentResolutionForTheRest()
        {
            // Result will be Int,Int,DateTime
            var root = new WithTheRest()
            {
                Boolean = false,
                Bytes = new byte[4] { 1, 2, 3, 4 },
                Guid = Guid.NewGuid(),
                Date = DateTime.Now,
                DateOffset = DateTimeOffset.Now,
                Time = TimeSpan.FromMinutes(100),
                Null = null
            };

            var generator = new JsonCodeGenerator("csharp");
            var classTypes = generator.GenerateClassTypesFromObject("Root", JsonExtensions.ToJObject(root))
                                      .ToLookup(x => x.Name);

            Assert.Equal(1, classTypes.Count());


            var clazz = classTypes["Root"].Single() as JsonCodeGenerator.ClassType;
            Assert.NotNull(clazz);

            Assert.Equal("Guid", clazz.Properties["Guid"].Name);
            Assert.False(clazz.Properties["Guid"].IsArray);
            Assert.True(clazz.Properties["Guid"].IsPrimitive);

            Assert.Equal("DateTimeOffset", clazz.Properties["Date"].Name);
            Assert.False(clazz.Properties["Date"].IsArray);
            Assert.True(clazz.Properties["Date"].IsPrimitive);

            Assert.Equal("DateTimeOffset", clazz.Properties["DateOffset"].Name);
            Assert.False(clazz.Properties["DateOffset"].IsArray);
            Assert.True(clazz.Properties["DateOffset"].IsPrimitive);

            Assert.Equal("TimeSpan", clazz.Properties["Time"].Name);
            Assert.False(clazz.Properties["Time"].IsArray);
            Assert.True(clazz.Properties["Time"].IsPrimitive);

            Assert.Equal("bool", clazz.Properties["Boolean"].Name);
            Assert.False(clazz.Properties["Boolean"].IsArray);
            Assert.True(clazz.Properties["Boolean"].IsPrimitive);

            Assert.Equal("object", clazz.Properties["Null"].Name);
            Assert.False(clazz.Properties["Null"].IsArray);
            Assert.True(clazz.Properties["Null"].IsPrimitive);

            Assert.Equal("byte", clazz.Properties["Bytes"].Name);
            Assert.True(clazz.Properties["Bytes"].IsArray);
            Assert.True(clazz.Properties["Bytes"].IsPrimitive);
        }

        [Fact]
        public void JsonCodeGenerator_WithDistinctInnerObject()
        {
            var obj = new WithInnerObject()
            {
                First = new WithIntsAndDateTimes() { Integer = 1, Long = (long)(int.MaxValue) + 1, Date = DateTime.Now },
                Second = new WithStrings() { Title = "test", Category = "category" }
            };

            var generator = new JsonCodeGenerator("csharp");
            var classTypes = generator.GenerateClassTypesFromObject("Root", JsonExtensions.ToJObject(obj))
                                      .ToLookup(x => x.Name);

            Assert.Equal(3, classTypes.Count());


            var clazz = classTypes["Root"].Single() as JsonCodeGenerator.ClassType;
            Assert.NotNull(clazz);

            var first = classTypes["FirstClass"].Single() as JsonCodeGenerator.ClassType;
            Assert.NotNull(first);

            Assert.Equal(3, first.Properties.Count);
            Assert.Equal("int", first.Properties["Integer"].Name);
            Assert.Equal("long", first.Properties["Long"].Name);
            Assert.Equal("DateTimeOffset", first.Properties["Date"].Name);

            Assert.True(first.Properties.All(x => !x.Value.IsArray && x.Value.IsPrimitive));

            var second = classTypes["SecondClass"].Single() as JsonCodeGenerator.ClassType;
            Assert.NotNull(second);

            Assert.Equal(2, second.Properties.Count);
            Assert.Equal("string", second.Properties["Title"].Name);
            Assert.Equal("string", second.Properties["Category"].Name);
            Assert.True(second.Properties.All(x => !x.Value.IsArray && x.Value.IsPrimitive));
        }

        [Fact]
        public void JsonCodeGenerator_WithArrayOfBasics()
        {
            var obj = new WithArrayOfBasics()
            {
                Ints = new int[] { 0, 1, 2 },
                Strings = new string[] { "test", "category" }
            };

            var generator = new JsonCodeGenerator("csharp");
            var classTypes = generator.GenerateClassTypesFromObject("Root", JsonExtensions.ToJObject(obj))
                                      .ToLookup(x => x.Name);

            Assert.Equal(1, classTypes.Count());

            var clazz = classTypes["Root"].Single() as JsonCodeGenerator.ClassType;
            Assert.NotNull(clazz);

            Assert.Equal("int", clazz.Properties["Ints"].Name);
            Assert.True(clazz.Properties["Ints"].IsArray);
            Assert.True(clazz.Properties["Ints"].IsPrimitive);

            Assert.Equal("string", clazz.Properties["Strings"].Name);
            Assert.True(clazz.Properties["Strings"].IsArray);
            Assert.True(clazz.Properties["Strings"].IsPrimitive);
        }


        [Fact]
        public void JsonCodeGenerator_WithArrayOfObjects()
        {
            var root = new WithArrayOfObjects()
            {
                Objects = new List<WithStrings> 
                {
                     new WithStrings() { Title = "test", Category = "category" },
                     new WithStrings() { Title = "test", Category = "category" },
                }
            };

            var generator = new JsonCodeGenerator("csharp");
            var classTypes = generator.GenerateClassTypesFromObject("Root", JsonExtensions.ToJObject(root))
                                      .ToLookup(x => x.Name);

            Assert.Equal(2, classTypes.Count());

            var clazz = classTypes["Root"].Single() as JsonCodeGenerator.ClassType;
            Assert.NotNull(clazz);

            var first = classTypes["Objects"].Single() as JsonCodeGenerator.ClassType;
            Assert.NotNull(first);

            Assert.Equal("Objects", clazz.Properties["Objects"].Name);
            Assert.True(clazz.Properties["Objects"].IsArray);
            Assert.False(clazz.Properties["Objects"].IsPrimitive);
        }

        [Fact]
        public void JsonCodeGenerator_WithArrayOfFloats()
        {
            var root = new WithArrayOfFloats()
            {
                Floats = new List<float> 
                {
                     -1.0f,
                     +1.0f,
                }
            };

            var generator = new JsonCodeGenerator("csharp");
            var classTypes = generator.GenerateClassTypesFromObject("Root", JsonExtensions.ToJObject(root))
                                      .ToLookup(x => x.Name);

            Assert.Equal(1, classTypes.Count());

            var clazz = classTypes["Root"].Single() as JsonCodeGenerator.ClassType;
            Assert.NotNull(clazz);

            Assert.Equal("float", clazz.Properties["Floats"].Name);
            Assert.True(clazz.Properties["Floats"].IsArray);
            Assert.True(clazz.Properties["Floats"].IsPrimitive);
        }
    }
}