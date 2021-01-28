using AutoFixture;
using Ei.Rp.DomainModels.Contracts;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Events;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Queries.Billing.InvoiceFiles;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.Stubs.CoreServices.Http.Session;

namespace EI.RP.DomainServices.UnitTests.Queries.Billing.InvoiceFiles
{
    [TestFixture]
    internal class InvoiceFileQueryEventBuilderTest : UnitTestFixture<InvoiceFileQueryEventBuilderTest.TestContext, InvoiceFileQueryEventBuilder>
    {
        public class TestContext : UnitTestContext<InvoiceFileQueryEventBuilder>
        {
            public InvoiceFileQueryEventBuilder Query { get; private set; }
            public InvoiceFileQuery InvoiceFileQuery { get; private set; }

            public string AccountNumber { get; private set; } = "1234";
            public string ReferenceDocNumber { get; private set; } = "5678";
            public string UserName { get; private set; } = "EISmart";
            public string Partner => "456789";

            public TestContext() : base(new Fixture().CustomizeDomainTypeBuilders())
            {
            }

            public TestContext WithAccountNumber(string accountNumber)
            {
                AccountNumber = accountNumber;
                return this;
            }

            public TestContext WithReferenceDocNumber(string referenceDocNumber)
            {
                ReferenceDocNumber = referenceDocNumber;
                return this;
            }

            public TestContext WithUserName(string userName)
            {
                UserName = userName;
                return this;
            }

            protected override InvoiceFileQueryEventBuilder BuildSut(AutoMocker autoMocker)
            {
                var domainFacade = new DomainFacade();
                domainFacade.SetUpMocker(autoMocker);

                var accountInfo = Fixture.Build<AccountInfo>()
                    .With(x => x.AccountNumber, AccountNumber)
                    .With(x => x.Partner, Partner)
                    .With(x=>x.PointReferenceNumber,Fixture.Create<ElectricityPointReferenceNumber>())
                    .Create();

                domainFacade.QueryResolver.Current.Setup(x =>
                        x.FetchAsync<AccountInfoQuery, AccountInfo>(It.IsAny<AccountInfoQuery>(), It.IsAny<bool>()))
                    .Returns(Task.FromResult(accountInfo.ToOneItemArray().AsEnumerable()));

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, UserName),
                    new Claim(ClaimTypes.NameIdentifier, "userId")
                };
                var identity = new ClaimsIdentity(claims, "TestAuthType");
                var claimsPrincipal = new ClaimsPrincipal(identity);
                var mock = autoMocker.GetMock<FakeSessionProviderStrategy>();
                mock.SetupGet(c => c.CurrentUserClaimsPrincipal)
                    .Returns(claimsPrincipal);
                autoMocker.Use<IUserSessionProvider>(mock.Object);
                

                InvoiceFileQuery = Fixture.Build<InvoiceFileQuery>()
                    .With(x => x.AccountNumber, AccountNumber)
                    .With(x => x.ReferenceDocNumber, ReferenceDocNumber)
                    .Create();

                Query = Fixture.Build<InvoiceFileQueryEventBuilder>()
                    .Create();

                return base.BuildSut(autoMocker);
            }
        }

        [Test]

        public async Task EventOnSuccess_Test()
        {
            var message = (await Context
                .WithAccountNumber("950187234")
                .WithReferenceDocNumber("1008425142")
                .Sut
                .BuildEventOnSuccess(Context.InvoiceFileQuery)) as EventApiEvent;

            Assert.NotNull(message);
            Assert.AreEqual(EventCategory.View, message.CategoryId);
            Assert.AreEqual(EventAction.LastOperationWasSuccessful, message.ActionId);
            Assert.AreEqual(Context.Partner, Convert.ToString(message.Partner));
            Assert.AreEqual(Context.AccountNumber, Convert.ToString(message.ContractAccount));
            Assert.AreEqual(Context.ReferenceDocNumber, message.Description);
            Assert.AreEqual(EventSubCategory.ViewBill, message.SubCategoryId);
        }

        [Test]

        public async Task EventOnError_Test()
        {
            var message = (await Context
                .WithAccountNumber("950187234")
                .WithReferenceDocNumber("1008425142")
                .WithUserName("EI Smart")
                .Sut
                .BuildEventOnError(Context.InvoiceFileQuery, null)) as EventApiEvent;

            Assert.NotNull(message);
            Assert.AreEqual(Context.UserName, message.Username);
            Assert.AreEqual(EventCategory.View, message.CategoryId);
            Assert.AreEqual(EventAction.LastOperationFailed, message.ActionId);
            Assert.AreEqual(Context.Partner, Convert.ToString(message.Partner));
            Assert.AreEqual(Context.AccountNumber, Convert.ToString(message.ContractAccount));
            Assert.AreEqual(Context.ReferenceDocNumber, message.Description);
            Assert.AreEqual(EventSubCategory.ViewBill, message.SubCategoryId);
        }

    }
}
