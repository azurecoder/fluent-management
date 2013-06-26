using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Types.Websites;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace Elastacloud.AzureManagement.Fluent.WasabiWeb.Test
{
    [TestFixture]
    public class TestEngine
    {
        #region Helper methods
        
        private List<WebsiteMetric> GetInitialMetric()
        {
            var list = new List<WebsiteMetric>();

            var metric = new WebsiteMetric()
                {
                    DisplayName = "tom",
                    EndTime = DateTime.UtcNow,
                    Name = "tom 1",
                    StartTime = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5)),
                    Total = 5,
                    Units = "elastaclouds"
                };
            list.Add(metric);
            return list;
        }

        private List<WebsiteMetric> AddSecondMetric()
        {
            var metrics = GetInitialMetric();
            var metric = new WebsiteMetric()
            {
                DisplayName = "bob",
                EndTime = DateTime.UtcNow,
                Name = "bob 1",
                StartTime = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5)),
                Total = 9,
                Units = "elastaclouds"
            };
            metrics.Add(metric);
            return new List<WebsiteMetric>(metrics);
        }

        private List<WebsiteMetric> AddThirdMetric()
        {
            var metrics = AddSecondMetric();
            var metric = new WebsiteMetric()
            {
                DisplayName = "bill",
                EndTime = DateTime.UtcNow,
                Name = "bill 1",
                StartTime = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5)),
                Total = 9,
                Units = "elastaclouds"
            };
            metrics.Add(metric);
            return new List<WebsiteMetric>(metrics);
        }

        #endregion

        [Test]
        public void WasabiWeb_WasabiRule_Create()
        {
            var rule = new WasabiWebRule("name", 3, 3);

            rule.MetricName.Should().Be("name");
            rule.IsLessThan.Should().Be(3);
            rule.IsGreaterThan.Should().Be(3);
        }

        [Test]
        public void WasabiRulesEngine_CreateSingle_ScaleUp()
        {
            // arrange
            var rule = new WasabiWebRule("tom", 3, 3);
            var engine = new WasabiWebRulesEngine("mysite", 3);
            engine.AddRule(rule);

            // act
            var response = engine.Scale(WasabiWebLogicalOperation.And, GetInitialMetric());
            // assert
            response.Should().Be(WasabiWebState.ScaleUp);
        }

        [Test]
        public void WasabiRulesEngine_CreateSingle_ScaleDown()
        {
            // arrange
            var rule = new WasabiWebRule("tom", 9, 6);
            var engine = new WasabiWebRulesEngine("mysite", 3);
            engine.AddRule(rule);

            // act
            var response = engine.Scale(WasabiWebLogicalOperation.And, GetInitialMetric());
            // assert
            response.Should().Be(WasabiWebState.ScaleDown);
        }

        [Test]
        public void WasabiRulesEngine_CreateSingle_Unchanged()
        {
            // arrange
            var rule = new WasabiWebRule("tom", 9, 6);
            var engine = new WasabiWebRulesEngine("mysite", 3);
            engine.AddRule(rule);

            // act
            var response = engine.Scale(WasabiWebLogicalOperation.And, GetInitialMetric());
            // assert
            response.Should().Be(WasabiWebState.ScaleDown);
        }

        [Test, ExpectedException(typeof(WasabiWebException))]
        public void WasabiRulesEngine_AddDuplicateRule()
        {
            var rule1 = new WasabiWebRule("tom", 3, 3);
            var rule2 = new WasabiWebRule("tom", 3, 3);
            var engine = new WasabiWebRulesEngine("mysite", 3);
            engine.AddRule(rule1);
            engine.AddRule(rule2);
        }

        [Test]
        public void WasabiRulesEngine_CreateManyWithAnd_ScaleUp()
        {
            // arrange - tom is 5 and bob is 9
            var rule1 = new WasabiWebRule("tom", 4, 2);
            var rule2 = new WasabiWebRule("bob", 8, 2);
            var engine = new WasabiWebRulesEngine("mysite", 3);
            engine.AddRule(rule1);
            engine.AddRule(rule2);

            // act
            var response = engine.Scale(WasabiWebLogicalOperation.And, AddSecondMetric());
            // assert
            response.Should().Be(WasabiWebState.ScaleUp);
        }

        [Test]
        public void WasabiRulesEngine_CreateManyWithAnd_ScaleDown()
        {
            // arrange - tom is 5 and bob is 9
            var rule1 = new WasabiWebRule("tom", 14, 6);
            var rule2 = new WasabiWebRule("bob", 18, 10);
            var engine = new WasabiWebRulesEngine("mysite", 3);
            engine.AddRule(rule1);
            engine.AddRule(rule2);

            // act
            var response = engine.Scale(WasabiWebLogicalOperation.And, AddSecondMetric());
            // assert
            response.Should().Be(WasabiWebState.ScaleDown);
        }

        [Test]
        public void WasabiRulesEngine_CreateManyWithAnd_Unchanged()
        {
            // arrange - tom is 5 and bob is 9
            var rule1 = new WasabiWebRule("tom", 14, 6);
            var rule2 = new WasabiWebRule("bob", 8, 6);
            var engine = new WasabiWebRulesEngine("mysite", 3);
            engine.AddRule(rule1);
            engine.AddRule(rule2);

            // act
            var response = engine.Scale(WasabiWebLogicalOperation.And, AddSecondMetric());
            // assert
            response.Should().Be(WasabiWebState.LeaveUnchanged);
        }

        [Test]
        public void WasabiRulesEngine_CreateManyWithOr_ScaleUp()
        {
            // arrange - tom is 5 and bob is 9
            var rule1 = new WasabiWebRule("tom", 4, 2);
            var rule2 = new WasabiWebRule("bob", 8, 2);
            var engine = new WasabiWebRulesEngine("mysite", 3);
            engine.AddRule(rule1);
            engine.AddRule(rule2);

            // act
            var response = engine.Scale(WasabiWebLogicalOperation.Or, AddSecondMetric());
            // assert
            response.Should().Be(WasabiWebState.ScaleUp);
        }

        [Test]
        public void WasabiRulesEngine_CreateManyWithOr_ScaleDown()
        {
            // arrange - tom is 5 and bob is 9
            var rule1 = new WasabiWebRule("tom", 14, 6);
            var rule2 = new WasabiWebRule("bob", 18, 10);
            var engine = new WasabiWebRulesEngine("mysite", 3);
            engine.AddRule(rule1);
            engine.AddRule(rule2);

            // act
            var response = engine.Scale(WasabiWebLogicalOperation.Or, AddSecondMetric());
            // assert
            response.Should().Be(WasabiWebState.ScaleDown);
        }

        [Test]
        public void WasabiRulesEngine_CreateManyWithOr_Unchanged()
        {
            // arrange - tom is 5 and bob is 9 bill is 
            var rule1 = new WasabiWebRule("tom", 14, 6);
            var rule2 = new WasabiWebRule("bob", 8, 6);
            var engine = new WasabiWebRulesEngine("mysite", 3);
            engine.AddRule(rule1);
            engine.AddRule(rule2);

            // act
            var response = engine.Scale(WasabiWebLogicalOperation.Or, AddSecondMetric());
            // assert
            response.Should().Be(WasabiWebState.LeaveUnchanged);
        }

        [Test, Ignore]
        public void WasabiRulesEngine_CreateManyWithAnd_ScaleUp3()
        {
            // arrange - tom is 5 and bob is 9 bill is 
            var rule1 = new WasabiWebRule("tom", 14, 6);
            var rule2 = new WasabiWebRule("bob", 8, 6);
            var rule3 = new WasabiWebRule("bill", 8, 6);
            var engine = new WasabiWebRulesEngine("mysite", 3);
            engine.AddRule(rule1);
            engine.AddRule(rule2);

            // act
            var response = engine.Scale(WasabiWebLogicalOperation.Or, AddThirdMetric());
            // assert
            response.Should().Be(WasabiWebState.LeaveUnchanged);
        }
    }
}
