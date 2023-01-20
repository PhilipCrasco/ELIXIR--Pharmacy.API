using ELIXIR.API.ERRORS;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("errors/{code}")]
    public class ErrorController : BaseApiController
    {
        public IActionResult Error(int code)
        {
            return new ObjectResult(new ApiResponse(code));

        }
    }
}
