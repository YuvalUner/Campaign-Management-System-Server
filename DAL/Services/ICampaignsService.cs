﻿using DAL.Models;

namespace DAL.Services;

public interface ICampaignsService
{
    Task<int> AddCampaign(Campaign campaign, int? campaignCreatorUserId);
    Task ModifyCampaign(Campaign campaign);
    Task<Guid?> GetCampaignGuid(int? campaignId);
}