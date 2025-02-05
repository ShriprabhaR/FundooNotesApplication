using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interface;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;

namespace ManagerLayer.Services
{
    public class LabelManager : ILabelManager
    {
        private readonly ILabelRepository label;
        private readonly FunDooDBContext context;

        public LabelManager(ILabelRepository label, FunDooDBContext context)
        {
            this.label = label;
            this.context = context;
        }

        public Label AddLabel(int UserId, LabelModel model)
        {
            return label.AddLabel(UserId, model);
        }
        public List<Label> GetLabel(int UserId)
        {
            return label.GetLabel(UserId);
        }
        public Label UpdateLabel(int LabelId, int UserId, LabelModel model)
        {
            return label.UpdateLabel(LabelId, UserId, model);
        }
        public bool DeleteLabel(int UserId, int LabelId)
        {
            return label.DeleteLabel(UserId, LabelId);
        }




    }
}
