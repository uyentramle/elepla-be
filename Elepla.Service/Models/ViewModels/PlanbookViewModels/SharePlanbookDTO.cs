using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.PlanbookViewModels
{
    public class SharePlanbookDTO
    {
        public string PlanbookId { get; set; }
        //public string SharedBy { get; set; }
        public List<PlanbookShareUserDTO> SharedTo { get; set; } // Danh sách người nhận chia sẻ và quyền của họ
    }

    public class PlanbookShareUserDTO
    {
        public string UserId { get; set; } // ID của người nhận chia sẻ
        public bool IsEdited { get; set; }  // Quyền chỉnh sửa cho người đó
    }
}
