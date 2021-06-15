using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RI.AppFramework.EntityModel
{
    public class TestUtilityLoadTestDetail
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public int HdrId { get; set; }
        public int MerchantId { get; set; }
        public int PosUserId { get; set; }
        public int PosAssignmentId { get; set; }
        public int PosId { get; set; }
        public string TxnNo { get; set; }
        public int PinDownloadId { get; set; }
        public int DownloadedPinId { get; set; }
        public int ProductId { get; set; }
        public string ResponseCode { get; set; }
        public bool IsDownloadCompleted { get; set; }
        public int DownloadResponseTime { get; set; }
        public bool IsConfirmed { get; set; }
        public int ConfirmationReponseTime { get; set; }
    }
}
