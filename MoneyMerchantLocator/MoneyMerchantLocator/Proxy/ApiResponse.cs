using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Proxy
{
    public class ApiResponse
    {
        private HttpResponseMessage response;

        private HttpRequestException exception;

        public HttpResponseMessage Response
        {
            get { return response; }
        }

        public ApiResponse(HttpResponseMessage response)
        {
            this.response = response;
        }

        public ApiResponse(HttpRequestException exception)
        {
            this.exception = exception;
        }

        public WebException InnerWebException
        {
            get
            {
                return exception?.InnerException as WebException;
            }
        }

        public bool Successful
        {
            get
            {
                return Response?.IsSuccessStatusCode == true;
            }
        }

        public string FormattedMessage
        {
            get
            {
                if (Successful)
                    return "OK";

                //
                if (IsConnectionError)
                    return "Failed contacting server. Please check network connectivity!";

                if (IsInternalServerError)
                    return "An internal server error occured.";

                if (IsBadRequest || IsForbidden || IsUnauthorized)
                {
                    var content = Response.Content.ReadAsStringAsync().Result;
                    if (IsForbidden)
                        return JsonConvert.DeserializeObject<string>(content);

                    return JsonConvert.DeserializeAnonymousType(content, new
                    {
                        Message = ""
                    }).Message;

                }

                return "Operation failed";
            }
        }

        public HttpStatusCode? StatusCode
        {
            get
            {
                return Response?.StatusCode;
            }
        }

        public bool IsInternalServerError
        {
            get
            {
                return StatusCode == HttpStatusCode.InternalServerError;
            }
        }

        public bool IsBadRequest
        {
            get
            {
                return StatusCode == HttpStatusCode.BadRequest;
            }
        }

        public bool IsUnauthorized
        {
            get
            {
                return StatusCode == HttpStatusCode.Unauthorized;
            }
        }

        public bool IsForbidden
        {
            get
            {
                return StatusCode == HttpStatusCode.Forbidden;
            }
        }

        public bool IsConnectionError
        {
            get
            {
                var innerEx = InnerWebException;
                if (innerEx != null)
                {
                    switch (innerEx.Status)
                    {
                        case WebExceptionStatus.SendFailure:
                        case WebExceptionStatus.ConnectFailure:
                        case WebExceptionStatus.UnknownError:
                            return true;
                    }
                }

                return false;
            }
        }
    }

    public class ApiResponse<TData> : ApiResponse
    {
        private TData _data;

        public ApiResponse(HttpResponseMessage response) : base(response)
        {
        }

        public ApiResponse(HttpRequestException exception) : base(exception)
        {
        }

        /// <summary>
        /// Gets the data for the response
        /// </summary>
        /// <returns></returns>
        public TData Data
        {
            get
            {
                return GetDataAsync().Result;
            }
        }

        /// <summary>
        /// Gets the data for the response asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<TData> GetDataAsync()
        {
            if (_data != null)
            {
                return _data;
            }

            //
            var content = await Response.Content.ReadAsStringAsync();
            _data =  JsonConvert.DeserializeObject<TData>(content);
            return _data;
        }
    }
}
