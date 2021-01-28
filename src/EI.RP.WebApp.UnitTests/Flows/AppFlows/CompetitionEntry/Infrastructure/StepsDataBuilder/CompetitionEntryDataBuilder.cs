using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using System.Linq;
using ScreenModel = EI.RP.WebApp.Flows.AppFlows.CompetitionEntry.Steps.CompetitionEntry.ScreenModel;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.CompetitionEntry.Infrastructure.StepsDataBuilder
{
    class CompetitionEntryDataBuilder
    {
        private readonly CommonElectricityAndGasAccountConfigurator _accountConfigurator;

        public CompetitionEntryDataBuilder(AppUserConfigurator appUserConfigurator)
        {
            //if need to support more than one do it explicitly
            _accountConfigurator = appUserConfigurator.ElectricityAndGasAccountConfigurators.Single();
        }


        public ScreenModel Create()
        {
            var builder = _accountConfigurator.DomainFacade.ModelsBuilder;
            var fixture = new Fixture();
            return builder
                .Build<ScreenModel>()
                .With(x => x.AccountNumber, _accountConfigurator.Model.AccountNumber)
                .With(x => x.Answer, fixture.Create<string>())
                .With(x => x.Consent, true)
                .With(x => x.Name, fixture.Create<string>())
                .With(x => x.Name, fixture.Create<string>())
                .With(x => x.Heading, fixture.Create<string>())
                .With(x => x.Description, fixture.Create<string>())
                .With(x => x.Description1, fixture.Create<string>())
                .With(x => x.Description2, fixture.Create<string>())
                .With(x => x.Description3, fixture.Create<string>())
                .With(x => x.QuestionPart1, fixture.Create<string>())
                .With(x => x.QuestionPart2, fixture.Create<string>())
                .With(x => x.QuestionPart3, fixture.Create<string>())
                .With(x => x.AnswerA, fixture.Create<string>())
                .With(x => x.AnswerB, fixture.Create<string>())
                .With(x => x.AnswerC, fixture.Create<string>())
                .With(x => x.TermAndConditionsUrl, "https://www.electricireland.ie/residential/competitions/terms-and-conditions/smart-home-bundle-competition")
                .With(x=> x.HasExistingEntry, false)
                .Create();
        }
    }
}