using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CMFL.MVVM.Class.Helper.Game.Tests
{
    [TestClass]
    public class GameIconHelperTests
    {
        [TestMethod]
        public void GetIconTest()
        {
            var icon1 = GameIconHelper.GetIcon("Bookshelf");

            Assert.AreEqual("/Assets/Images/Icons/Bookshelf.png", icon1);
        }

        [TestMethod]
        public void GetIconIndexTest()
        {
            var index1 = GameIconHelper.GetIconIndex("Furnace_On");
            var index2 = GameIconHelper.GetIconIndex(string.Empty);

            Assert.AreEqual(0, index1);
            Assert.AreEqual(-1, index2);
        }
    }
}