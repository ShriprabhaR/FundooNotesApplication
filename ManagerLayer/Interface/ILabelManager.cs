using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;

namespace ManagerLayer.Interface
{
    public interface ILabelManager
    {
        public Label AddLabel(int UserId, LabelModel model);
        public List<Label> GetLabel(int UserId);
        public Label UpdateLabel(int LabelId, int UserId, LabelModel model);
        public bool DeleteLabel(int UserId, int LabelId);
    }
}
