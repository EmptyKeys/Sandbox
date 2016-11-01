using System;
using GameLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest
    {
        private BasicViewModel viewModel;

        [TestInitialize]
        public void Init()
        {
            viewModel = new BasicViewModel();
        }

        [TestMethod]
        public void TestClickMe()
        {
            Assert.AreEqual("Hello GDS!", viewModel.Text);
            viewModel.ClickMeCommand.Execute(null);
            Assert.AreEqual("It works!", viewModel.Text);
        }
    }
}
