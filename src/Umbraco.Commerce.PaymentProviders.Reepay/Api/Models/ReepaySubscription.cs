using Newtonsoft.Json;
using Umbraco.Commerce.PaymentProviders.Reepay.Api.Enums;

namespace Umbraco.Commerce.PaymentProviders.Reepay.Api.Models
{
    public class ReepaySubscription
    {
        [JsonProperty("handle")]
        public string Handle { get; set; }

        [JsonProperty("customer")]
        public string Customer { get; set; }

        [JsonProperty("plan")]
        public string Plan { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("test")]
        public bool Test { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("expires")]
        public string Expires { get; set; }

        [JsonProperty("reactivated")]
        public string Reactivated { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("activated")]
        public DateTime? Activated { get; set; }

        [JsonProperty("renewing")]
        public bool Renewing { get; set; }

        [JsonProperty("plan_version")]
        public int PlanVersion { get; set; }

        [JsonProperty("amount_incl_vat")]
        public bool? AmountInclVat { get; set; }

        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }

        [JsonProperty("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("is_cancelled")]
        public bool IsCancelled { get; set; }

        [JsonProperty("in_trial")]
        public bool InTrial { get; set; }

        [JsonProperty("has_started")]
        public bool HasStarted { get; set; }

        [JsonProperty("renewal_count")]
        public int RenewalCount { get; set; }

        [JsonProperty("plan_version")]
        public int Plan_version { get; set; }

        [JsonProperty("amount_incl_vat")]
        public bool Amount_incl_vat { get; set; }

        [JsonProperty("start_date")]
        public string Start_date { get; set; }

        [JsonProperty("end_date")]
        public string End_date { get; set; }

        [JsonProperty("grace_duration")]
        public int Grace_duration { get; set; }
    }



    public class CreateSubscriptionRequest : ReepaySubscription
    {
        [JsonProperty("metadata")]
        public dynamic Metadata { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("create_customer")]
        public CreateCustomer Create_customer { get; set; }

        [JsonProperty("generate_handle")]
        public bool Generate_handle { get; set; }

        [JsonProperty("no_trial")]
        public bool No_trial { get; set; }

        [JsonProperty("no_setup_fee")]
        public bool No_setup_fee { get; set; }

        [JsonProperty("trial_period")]
        public string Trial_period { get; set; }

        [JsonProperty("coupon_codes")]
        public IList<string> Coupon_codes { get; set; }

        [JsonProperty("subscription_discounts")]
        public IList<CreateSubscriptionDiscount> Subscription_discounts { get; set; }

        [JsonProperty("add_ons")]
        public IList<dynamic> Add_ons { get; set; }

        [JsonProperty("additional_costs")]
        public IList<dynamic> Additional_costs { get; set; }

        [JsonProperty("signup_method")]
        public string Signup_method { get; set; }

        [JsonProperty("conditional_create")]
        public bool Conditional_create { get; set; }

        public CreateSubscriptionRequest()
        {
            Signup_method = SubscriptionSignupMethodEnum.source.ToString();
        }

        //public CreateSubscriptionRequest(PaymentMethod paymentMethod) : this()
        //{
        //    try
        //    {
        //        Test = paymentMethod.DynamicProperty<bool>().Test;
        //    }
        //    catch (Exception)
        //    {
        //        Test = false;
        //    }
        //}
    }

    public class CreateCustomer : CustomerAddress
    {
        [JsonProperty("handle")]
        public string Handle { get; set; }

        [JsonProperty("test")]
        public bool Test { get; set; }

        [JsonProperty("metadata")]
        public dynamic Metadata { get; set; }

        [JsonProperty("generate_handle")]
        public bool Generate_handle { get; set; }

        //public CreateCustomer() { }

        //public CreateCustomer(PaymentMethod paymentMethod)
        //{
        //    try
        //    {
        //        Test = paymentMethod.DynamicProperty<bool>().Test;
        //    }
        //    catch (Exception)
        //    {
        //        Test = false;
        //    }
        //}
    }

    public class CustomerAddress : AddressBase
    {
        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("vat")]
        public string Vat { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("first_name")]
        public string First_name { get; set; }

        [JsonProperty("last_name")]
        public string Last_name { get; set; }
    }

    public class AddressBase
    {
        [JsonProperty("address2")]
        public string Address2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("postal_code")]
        public string Postal_code { get; set; }
    }

}
