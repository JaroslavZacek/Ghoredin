using System;
using System.Collections.Generic;
using System.Text;

using Ghoredin.Application.Campaigns;
using Ghoredin.Application.Users;
using Ghoredin.Domain.Journal;

namespace Ghoredin.Application.Journal
{
    public class JournalService
    {
        private readonly IJournalRepository _journalRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignAuthorizationService _campaignAuthorizationService;
        private readonly ICurrentUserService _currentUserService;

        public JournalService(
            IJournalRepository journalRepository,
            ICampaignRepository campaignRepository,
            ICampaignAuthorizationService campaignAuthorizationService,
            ICurrentUserService currentUserService)
        {
            _journalRepository = journalRepository;
            _campaignRepository = campaignRepository;
            _campaignAuthorizationService = campaignAuthorizationService;
            _currentUserService = currentUserService;
        }

        

    }
}
