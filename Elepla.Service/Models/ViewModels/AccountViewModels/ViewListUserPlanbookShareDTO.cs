using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.AccountViewModels
{
    public class ViewListUserPlanbookShareDTO : ViewListUserToPlanbookShareDTO
    {
        public bool IsEdited { get; set; }
        public bool IsOwner { get; set; }
    }

    public class ViewListUserToPlanbookShareDTO
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string GoogleEmail { get; set; }
        public string FacebookEmail { get; set; }
        public string Avatar { get; set; }
    }
}
