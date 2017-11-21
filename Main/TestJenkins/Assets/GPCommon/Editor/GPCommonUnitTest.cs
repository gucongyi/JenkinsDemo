#if UNIT_TEST

using System.Collections.Generic;
using NUnit.Framework;

namespace GPCommon
{
    [TestFixture]
    public class GPCommonUnitTest
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void IsLegalVariableName()
        {
            string f1 = "1jdfigh";
            string f2 = "2djhkf";
            string f3 = "jdfi%gh1";
            string f4 = "sdf.2";
            string f5 = "1s_df";

            Assert.AreEqual(false, EditorHelper.IsLegalVariableName(f1));
            Assert.AreEqual(false, EditorHelper.IsLegalVariableName(f2));
            Assert.AreEqual(false, EditorHelper.IsLegalVariableName(f3));
            Assert.AreEqual(false, EditorHelper.IsLegalVariableName(f4));
            Assert.AreEqual(false, EditorHelper.IsLegalVariableName(f5));

            string t1 = "f2g";
            string t2 = "sdf2";
            string t3 = "sdf2gds";

            Assert.AreEqual(true, EditorHelper.IsLegalVariableName(t1));
            Assert.AreEqual(true, EditorHelper.IsLegalVariableName(t2));
            Assert.AreEqual(true, EditorHelper.IsLegalVariableName(t3));
        }

        [Test]
        public void CheckDuplicated()
        {
            List<string> s1 = new List<string>() {"1", "2", "3", "4", "5"};
            List<string> s2 = new List<string>() {"2", "2", "3", "2", "3"};

            string d;

            Assert.AreEqual(false, CommonUtils.CheckDuplicated(s1, out d));
            Assert.AreEqual(null, d);

            Assert.AreEqual(true, CommonUtils.CheckDuplicated(s2, out d));
            Assert.AreEqual("2", d);
        }
    }
}

#endif