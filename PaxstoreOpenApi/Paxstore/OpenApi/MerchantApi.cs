using System;

using RestSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using Paxstore.OpenApi.Base;
using Paxstore.OpenApi.Model;
using Paxstore.OpenApi.Validator.Merchant;

namespace Paxstore.OpenApi
{
    public class MerchantApi: BaseApi
    {
        private const string SEARCH_MERCHANT_URL = "/v1/3rdsys/merchants";
	    private const string GET_MERCHANT_URL = "/v1/3rdsys/merchants/{merchantId}";
	    private const string CREATE_MERCHANT_URL = "/v1/3rdsys/merchants";
	    private const string UPDATE_MERCHANT_URL = "/v1/3rdsys/merchants/{merchantId}";
	    private const string ACTIVATE_MERCHANT_URL = "/v1/3rdsys/merchants/{merchantId}/active";
	    private const string DISABLE_MERCHANT_URL = "/v1/3rdsys/merchants/{merchantId}/disable";
	    private const string DELETE_MERCHANT_URL = "/v1/3rdsys/merchants/{merchantId}";

        public MerchantApi(string baseUrl, string apiKey, string apiSecret) : base(baseUrl, apiKey, apiSecret){

        }

        public Result<PagedMerchant>  SearchMerchant(int pageNo, int pageSize, MerchantSearchOrderBy orderBy, String name, MerchantStatus status) {
            IList<string> validationErrs = ValidatePageSizeAndPageNo(pageSize, pageNo);
            if (validationErrs.Count > 0)
            {
                return new Result<PagedMerchant>(validationErrs);
            }
            RestRequest request = new RestRequest(SEARCH_MERCHANT_URL, Method.GET);
            request.AddParameter(Constants.PAGINATION_PAGE_NO, pageNo.ToString());
            request.AddParameter(Constants.PAGINATION_PAGE_LIMIT, pageSize.ToString());
            request.AddParameter("orderBy", GetOrderValue(orderBy));
            request.AddParameter("name", name);
            request.AddParameter("status", GetStatusValue(status));
            var responseContent = Execute(request);
            MerchantPageResponse merchantPageDTO = JsonConvert.DeserializeObject<MerchantPageResponse>(responseContent);
            Result<PagedMerchant> result = new Result<PagedMerchant>(merchantPageDTO);
            return result;
        }

        public Result<Merchant>  GetMerchant(long merchantId) {
            List<string> validationErrs = ValidateId(merchantId, "parameterMerchantIdInvalid");
            if(validationErrs.Count>0) {
                return new Result<Merchant>(validationErrs);
            }
            RestRequest request = new RestRequest(GET_MERCHANT_URL, Method.GET);
            request.AddUrlSegment("merchantId", merchantId);
            var responseContent = Execute(request);
            MerchantResponse merchantResponse = JsonConvert.DeserializeObject<MerchantResponse>(responseContent);
            Result<Merchant> result = new Result<Merchant>(merchantResponse);
            return result;
        }

        public Result<Merchant>  CreateMerchant(MerchantCreateRequest merchantCreateRequest) {
            List<string> validationErrs = ValidateCreate(merchantCreateRequest, new MerchantCreateValidator(),"merchantCreateRequestIsNull");
            if(validationErrs.Count>0){
                return new Result<Merchant>(validationErrs);
            }
            RestRequest request = new RestRequest(CREATE_MERCHANT_URL, Method.POST);
            var merchantJson = JsonConvert.SerializeObject(merchantCreateRequest);
            request.AddParameter(Constants.CONTENT_TYPE_JSON, merchantJson, ParameterType.RequestBody);
            var responseContent = Execute(request);
            MerchantResponse merchantResponse = JsonConvert.DeserializeObject<MerchantResponse>(responseContent);
            Result<Merchant> result = new Result<Merchant>(merchantResponse);
            return result;
        }
        
        public Result<Merchant>  UpdateMerchant(long merchantId, MerchantUpdateRequest merchantUpdateRequest) {
            IList<string> validationErrs = ValidateUpdate(merchantId, merchantUpdateRequest, new MerchantUpdateValidator(), "merchantIdInvalid", "merchantUpdateRequestIsNull");
            if(validationErrs.Count>0){
                return new Result<Merchant>(validationErrs);
            }
            RestRequest request = new RestRequest(UPDATE_MERCHANT_URL, Method.PUT);
            var merchantJson = JsonConvert.SerializeObject(merchantUpdateRequest);
            request.AddParameter(Constants.CONTENT_TYPE_JSON, merchantJson, ParameterType.RequestBody);
            request.AddUrlSegment("merchantId",merchantId);
            var responseContent = Execute(request);
            MerchantResponse merchantResponse = JsonConvert.DeserializeObject<MerchantResponse>(responseContent);
            Result<Merchant> result = new Result<Merchant>(merchantResponse);
            return result;
        }
        
        public Result<string> ActivateMerchant(long merchantId) {

            IList<string> validationErrs = ValidateId(merchantId, "parameterMerchantIdInvalid");
            if (validationErrs.Count > 0){
                return new Result<string>(validationErrs);
            }
            RestRequest request = new RestRequest(ACTIVATE_MERCHANT_URL, Method.PUT);
            request.AddUrlSegment("merchantId",merchantId);
            var responseContent = Execute(request);
            EmptyResponse emptyResponse = JsonConvert.DeserializeObject<EmptyResponse>(responseContent);
            Result<string> result = new Result<string>(emptyResponse);
            return result;
        }
        
        public Result<string> DisableMerchant(long merchantId) {
            List<string> validationErrs = ValidateId(merchantId, "parameterMerchantIdInvalid");
            if(validationErrs.Count>0) {
                return new Result<string>(validationErrs);
            }
            RestRequest request = new RestRequest(DISABLE_MERCHANT_URL, Method.PUT);
            request.AddUrlSegment("merchantId",merchantId);
            var responseContent = Execute(request);
            EmptyResponse emptyResponse = JsonConvert.DeserializeObject<EmptyResponse>(responseContent);
            Result<string> result = new Result<string>(emptyResponse);
            return result;
        }
        
        public Result<string> DeleteMerchant(long merchantId) {
            List<string> validationErrs = ValidateId(merchantId, "parameterMerchantIdInvalid");
            if(validationErrs.Count>0) {
                return new Result<string>(validationErrs);
            }
            RestRequest request = new RestRequest(DELETE_MERCHANT_URL, Method.DELETE);
            request.AddUrlSegment("merchantId",merchantId);
            var responseContent = Execute(request);
            EmptyResponse emptyResponse = JsonConvert.DeserializeObject<EmptyResponse>(responseContent);
            Result<string> result = new Result<string>(emptyResponse);
            return result;
        } 


        string GetStatusValue(MerchantStatus status)
        {
            switch (status)
            {
                case MerchantStatus.Active:
                    return "A";
                case MerchantStatus.Inactive:
                    return "P";
                case MerchantStatus.Suspend:
                    return "S";
                case MerchantStatus.All:
                    return null;
            }
            return null;
        }

        string GetOrderValue(MerchantSearchOrderBy order)
        {
            switch (order)
            {
                case MerchantSearchOrderBy.Name:
                    return "name";
                case MerchantSearchOrderBy.Contact:
                    return "contact";
                case MerchantSearchOrderBy.Phone:
                    return "phone";
            }
            return "name";
        }
    }

    public enum MerchantSearchOrderBy
    {
        Name,
        Contact,
        Phone
    }

    public enum MerchantStatus
    {
        All,
        Active,
        Inactive,
        Suspend
    }
}