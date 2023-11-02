using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjBobcat.Class.Model;
using System.Threading.Tasks;

namespace CMFL.MVVM.Class.Helper.Game.Tests
{
    [TestClass]
    public class BMCLHelperTests
    {
        [TestMethod]
        public async Task GetJavaDownloadLinksTest()
        {
            var result = await BMCLHelper.GetJavaDownloadLinks();

            Assert.AreEqual(TaskResultStatus.Success, result.TaskStatus);
            Assert.AreEqual(2, result.Value.Count);
        }
    }
}