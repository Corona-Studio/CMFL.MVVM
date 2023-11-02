using CMFL.MVVM.Class.Helper.Other;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace CMFL.MVVM.Class.Helper.Game.Tests
{
    [TestClass]
    public class MojangApiTests
    {
        [TestMethod]
        public async Task GetSkinUrlTest()
        {
            var result = await MojangApi.GetSkinUrl("bd847c29486144d186ce6ccd6227491b");

            Assert.AreNotEqual(null, result);
        }

        [TestMethod]
        public async Task GetUuidByNameAndTimeStampTest()
        {
            var result = await MojangApi.GetUuidByNameAndTimeStamp("laolarou", TimeHelper.GetTimeStampTen());

            Assert.AreNotEqual(null, result);
        }

        [TestMethod]
        public async Task ApiCheckTest()
        {
            var result = await MojangApi.ApiCheck();

            Assert.AreEqual(true, result.Any());
        }
    }
}