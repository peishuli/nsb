using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Lhi.NsbDemo.Orders.Api.Models
{
    public class LhiActionResult<T>
    {
        private T _data;
        public int StatusCode { get; set; }
        public Dictionary<string, string> CustomResponseHeaders { get; set; }
        public LhiActionResult(T data)
        {
            CustomResponseHeaders = new Dictionary<string, string>();
            StatusCode = StatusCodes.Status201Created;

            this._data = data;
        }
    }
}
