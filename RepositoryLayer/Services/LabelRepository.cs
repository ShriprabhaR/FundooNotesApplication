using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Services
{
    public class LabelRepository : ILabelRepository
    {
        private readonly FunDooDBContext context;

        public LabelRepository(FunDooDBContext context)
        {
            this.context = context;
        }

        public Label AddLabel(int userId, LabelModel model)
        {
            try
            {
                var existingLabel = context.Labels.FirstOrDefault(x => x.LabelName == model.LabelName && x.UserId == userId);

                if (existingLabel == null)
                {
                    Label label = new Label();

                    label.UserId = userId;
                    label.LabelName = model.LabelName;
                    label.NotesId = model.NotesId;

                    context.Labels.Add(label);
                    context.SaveChanges();
                    return label;
                }
                else
                {
                    Label label1 = new Label();
                    label1.LabelName = model.LabelName;
                    label1.UserId = userId;
                    label1.NotesId = model.NotesId;

                    context.Labels.Add(label1);
                    context.SaveChanges();
                    return label1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Label> GetLabel(int UserId)
        {
            try
            {
                var label = context.Labels.Select(x => x).ToList();
                return label;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Label UpdateLabel(int LabelId, int UserId, LabelModel model)
        {
            try
            {
                var labels = context.Labels.FirstOrDefault(x => x.UserId == UserId && x.LabelId == LabelId);

                if (labels == null)
                {
                    return null;
                }
                else
                {
                    labels.LabelName = model.LabelName;
                    context.Update(labels);
                    context.SaveChanges();
                    return labels;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteLabel(int UserId, int LabelId)
        {
            try
            {
                var labels = context.Labels.FirstOrDefault(x => x.UserId == UserId && x.LabelId == LabelId);

                if (labels == null)
                {
                    return false;
                }
                else
                {
                    context.Remove(labels);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
