using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmcUrm.Dtos;
using EI.RP.DataServices;
using EI.RP.DomainModels.SpecimenBuilders;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.Commands.Users.Membership.CreatePassword;
using EI.RP.DomainServices.Commands.Users.Session.CreateUserSession;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.TestServices;
using Moq;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Commands.Users.Membership.CreatePassword
{
	internal class CreatePasswordCommandHandlerTest : UnitTestFixture<CreatePasswordCommandHandlerTest.TestContext
		,
		CreatePasswordCommandHandler>
	{
		public class TestContext : UnitTestContext<CreatePasswordCommandHandler>
		{
			public TestContext() : base(new Fixture().CustomizeDomainTypeBuilders())
			{
			}
		}


		[Test]
		public async Task Execute_AdaptsPasswordToSapFormat()
		{
			var cmd = new CreatePasswordCommand("!a",Context.Fixture.Create<string>(), Context.Fixture.Create<string>(), Context.Fixture.Create<EmailAddress>().ToString(),"!b");
			string expectedPassword = cmd.NewPassword.AdaptToSapPasswordFormat();


			Context.AutoMocker.GetMock<ISapRepositoryOfCrmUmcUrm>().Setup(t =>
				t.Update(It.Is<UserRequestActivationRequestDto>(x => x.Password == expectedPassword), true))
				.Returns(Task.CompletedTask);

			Context.AutoMocker.GetMock<ICommandHandler<CreateUserSessionCommand>>().Setup(t =>
					t.ExecuteAsync(It.Is<CreateUserSessionCommand>(x => x.Password == expectedPassword)))
				.Returns(Task.CompletedTask);
			await Context.Sut.ExecuteAsync(cmd);

			Context.AutoMocker.VerifyAll();
		}
	}
}