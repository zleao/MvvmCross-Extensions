using MvxExtensions.Models;

namespace Playground.Forms.Core.Models
{
    public class CaseTest : Model
    {
        #region Properties

        private int _id;
        public int Id
        {
            get => _id;
            private set => SetProperty(ref _id, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            private set => SetProperty(ref _name, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            private set => SetProperty(ref _description, value);
        }

        #endregion

        #region Constructor

        public CaseTest(int id, string name, string description)
        {
            _id = id;
            _name = name;
            _description = description;
        }

        #endregion
    }
}
