using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NUnit.Framework;

namespace Manipulation
{
    [TestFixture]
    public class Visualizer_Tests
    {
        private static IEnumerable<TestCaseData> GetFormCenter_CaseData
        {
            get
            {
                var form = new Form();
                form.Location = new Point(0, 0);
                form.Size = new Size(width: 50, height: 50);
                

                var expected = new PointF(25, 25);
                yield return new TestCaseData(form, expected);
            }
        }

        [TestCaseSource("GetFormCenter_CaseData")]
        public void GetFormCenterPointF_Test(Form form, PointF expectedResult)
        {
            var result = VisualizerTask
                .GetFormCenterPointF(form);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
