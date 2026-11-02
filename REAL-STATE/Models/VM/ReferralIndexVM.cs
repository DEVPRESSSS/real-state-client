namespace REAL_STATE.Models.VM
{
    public class ReferralIndexVM
    {
        public List<ReferralRowVM> Referrals { get; set; } = new();
        public ReferralRowVM? Latest { get; set; }
    }
}