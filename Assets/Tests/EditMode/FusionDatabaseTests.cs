using NUnit.Framework;
using NUnit.Framework.Internal;

namespace FusionDatabaseTests
{
    [TestFixture]
    public class EmptyCell
    {
        private FusionDatabase db;

        [SetUp]
        public void SetUp()
        {
            db = new FusionDatabase();
        }

        [TestCase(PlantType.Sunflower)]
        [TestCase(PlantType.GreenGourd)]
        public void EmptyCell_WhenPlantInHand_ReturnsIncoming(PlantType plant)
        {
            var result = db.GetPlantResult(plant, null, null);
            Assert.AreEqual(plant, result);
        }

    }

    [TestFixture]
    public class OneExisting_NormalIncoming
    {
        private FusionDatabase db;

        [SetUp]
        public void SetUp()
        {
            db = new FusionDatabase();
        }

        [TestCase(PlantType.Sunflower, null, PlantType.Pumpkin)]
        [TestCase(PlantType.Sunflower, PlantType.Pumpkin, null)]
        [TestCase(PlantType.Peashooter, null, PlantType.Pumpkin)]
        [TestCase(PlantType.Peashooter, PlantType.Pumpkin, null)]
        public void OnExisting_NormalIncoming_ReturnsInComing(PlantType incoming, PlantType? primaryExisting, PlantType? otherExisting)
        {
            var result = db.GetPlantResult(incoming, primaryExisting, otherExisting);
            Assert.AreEqual(incoming, result);
        }

        [TestCase(PlantType.Peashooter, null, PlantType.Repeater, PlantType.Splitpea)]
        [TestCase(PlantType.Sunflower, PlantType.Sunflower, null, PlantType.Twinflower)]
        public void OnExisting_NormalIncoming_CanFuse_ReturnsFused(PlantType incoming, PlantType? primaryExisting, PlantType? otherExisting, PlantType expected)
        {
            var result = db.GetPlantResult(incoming, primaryExisting, otherExisting);
            Assert.AreEqual(expected, result);
        }

        [TestCase(PlantType.Splitpea, null, PlantType.Sunflower)]
        [TestCase(PlantType.Splitpea, PlantType.Twinflower, null)]
        public void OnExisting_NormalIncoming_NoFuse_ReturnNull(PlantType incoming, PlantType? primaryExisting, PlantType? otherExisting)
        {
            var result = db.GetPlantResult(incoming, primaryExisting, otherExisting);
            Assert.IsNull(result);
        }

    }

    [TestFixture]
    public class OneExisting_SupportIncoming
    {
        private FusionDatabase db;

        [SetUp]
        public void SetUp()
        {
            db = new FusionDatabase();
        }

        [TestCase(PlantType.Pumpkin, null, PlantType.Peashooter)]
        [TestCase(PlantType.Pumpkin, PlantType.Peashooter, null)]
        [TestCase(PlantType.PeaVine, null, PlantType.Peashooter)]
        [TestCase(PlantType.PeaVine, PlantType.Peashooter, null)]
        public void OnExisting_SupportIncoming_ReturnsInComing(PlantType incoming, PlantType? primaryExisting, PlantType? otherExisting)
        {
            var result = db.GetPlantResult(incoming, primaryExisting, otherExisting);
            Assert.AreEqual(incoming, result);
        }

        [TestCase(PlantType.GreenGourd, null, PlantType.GreenGourd, PlantType.XVine)]
        [TestCase(PlantType.GreenGourd, PlantType.GreenGourd, null, PlantType.XVine)]
        public void OnExisting_SupportIncoming_CanFuse_ReturnsFused(PlantType incoming, PlantType? primaryExisting, PlantType? otherExisting, PlantType expected)
        {
            var result = db.GetPlantResult(incoming, primaryExisting, otherExisting);
            Assert.AreEqual(expected, result);
        }

        [TestCase(PlantType.GreenGourd, null, PlantType.XVine)]
        [TestCase(PlantType.GreenGourd, PlantType.XVine, null)]
        public void OnExisting_SupportIncoming_NoFuse_ReturnNull(PlantType incoming, PlantType? primaryExisting, PlantType? otherExisting)
        {
            var result = db.GetPlantResult(incoming, primaryExisting, otherExisting);
            Assert.IsNull(result);
        }
    }


    [TestFixture]
    public class FullCell_NormalIncoming
    {
        private FusionDatabase db;

        [SetUp]
        public void SetUp()
        {
            db = new FusionDatabase();
        }

        [TestCase(PlantType.Sunflower, PlantType.Sunflower, PlantType.Pumpkin, PlantType.Twinflower)]
        [TestCase(PlantType.Peashooter, PlantType.Repeater, PlantType.XVine, PlantType.Splitpea)]
        [TestCase(PlantType.Peashooter, PlantType.Peashooter, PlantType.Pumpkin, PlantType.Repeater)]
        [TestCase(PlantType.Peashooter, PlantType.Twinflower, PlantType.Pumpkin, PlantType.GreenGourd)]
        [TestCase(PlantType.Sunflower, PlantType.PeaVine, PlantType.Sunflower, PlantType.Twinflower)]
        [TestCase(PlantType.Peashooter, PlantType.XVine, PlantType.Repeater, PlantType.Splitpea)]
        [TestCase(PlantType.Peashooter, PlantType.Pumpkin, PlantType.Peashooter, PlantType.GreenGourd)]
        public void HasFull_NormalIncoming_CanFuse_ReturnsFused(PlantType incoming, PlantType? primaryExisting, PlantType? otherExisting, PlantType expected)
        {
            var result = db.GetPlantResult(incoming, primaryExisting, otherExisting);
            Assert.AreEqual(expected, result);
        }

        [TestCase(PlantType.Sunflower, PlantType.Peashooter, PlantType.Pumpkin)]
        [TestCase(PlantType.Sunflower, PlantType.Pumpkin, PlantType.Peashooter)]
        public void HasFull_NormalIncoming_NoFuse_ReturnsNull(PlantType incoming, PlantType? primaryExisting, PlantType? otherExisting)
        {
            var result = db.GetPlantResult(incoming, primaryExisting, otherExisting);
            Assert.IsNull(result);
        }
    }

    [TestFixture]
    public class FullCell_SupportIncoming
    {
        private FusionDatabase db;

        [SetUp]
        public void SetUp()
        {
            db = new FusionDatabase();
        }

        [TestCase(PlantType.GreenGourd, PlantType.Sunflower, PlantType.GreenGourd, PlantType.XVine)]
        [TestCase(PlantType.GreenGourd, PlantType.GreenGourd, PlantType.Peashooter, PlantType.XVine)]
        [TestCase(PlantType.GreenGourd, PlantType.GreenGourd, PlantType.Twinflower, PlantType.XVine)]
        [TestCase(PlantType.GreenGourd, PlantType.Peashooter, PlantType.GreenGourd, PlantType.XVine)]
        public void HasFull_SupportIncoming_CanFuse_ReturnsFused(PlantType incoming, PlantType? primaryExisting, PlantType? otherExisting, PlantType expected)
        {
            var result = db.GetPlantResult(incoming, primaryExisting, otherExisting);
            Assert.AreEqual(expected, result);
        }

        [TestCase(PlantType.Pumpkin, PlantType.Peashooter, PlantType.Pumpkin)]
        [TestCase(PlantType.Pumpkin, PlantType.Pumpkin, PlantType.Peashooter)]
        [TestCase(PlantType.Pumpkin, PlantType.Twinflower, PlantType.XVine)]
        [TestCase(PlantType.Pumpkin, PlantType.XVine, PlantType.Splitpea)]
        public void HasFull_SupportIncoming_NoFuse_ReturnsNull(PlantType incoming, PlantType? primaryExisting, PlantType? otherExisting)
        {
            var result = db.GetPlantResult(incoming, primaryExisting, otherExisting);
            Assert.IsNull(result);
        }
    }
}