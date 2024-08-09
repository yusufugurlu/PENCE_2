using API.TeknofestNLPApp.Common;

namespace API.TeknofestNLPApp.Dtos
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rate { get; set; }
        public string CommentTitle { get; set; }
        public string LastModifiedDate { get; set; }
        public string CommentDateISOtype { get; set; }
        public int ReviewLikeCount { get; set; }
        public bool UserLiked { get; set; }
        public string UserFullName { get; set; }
        public string SellerName { get; set; }
        public LabelStatus Label { get; set; }
        public string AppType { get; set; }

        public string Status
        {
            get
            {
                switch (Label)
                {
                    case LabelStatus.UseFull:
                        return "success";
                    case LabelStatus.UnUsefull:
                        return "danger";
                }
                return "danger";
            }
        }

        public string LabelText
        {
            get {
                switch (Label)
                {
                    case LabelStatus.UseFull:
                        return "Faydalı";
                    case LabelStatus.UnUsefull:
                        return "Faydasız";
                }
                return "Faydasız"; }

        }

    }
}
