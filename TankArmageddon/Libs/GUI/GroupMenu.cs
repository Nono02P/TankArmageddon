using Microsoft.Xna.Framework;

namespace TankArmageddon.GUI
{
    interface IIntegrableMenu
    {
        bool Selected { get; set; }
        Color Color_Selected { get; set;  }
        Color ColorBck_Selected { get; set; }
        Color Color_Default { get; set; }
        Color ColorBck_Default { get; set; }
    }

    class GroupMenu : Group
    {
        #region Variables privées
        private int _currentSelection;
        private Vector2 _position;
        private Color _selectedTextColor = Color.Yellow;
        private Color _selectedTextColorBck = Color.Red;
        private Color _unselectedTextColor = Color.Gray;
        private Color _unselectedTextColorBck = Color.Red;
        #endregion

        #region Propriétés
        // sur changement, créer un event
        public int CurrentSelection { get { return _currentSelection; } set { _currentSelection = (Elements.Count + value) % Elements.Count; ; } }
        public Color SelectedTextColor { get { return _selectedTextColor; } set { _selectedTextColor = value; RefreshColors(); } }
        public Color SelectedTextColorBck { get { return _selectedTextColorBck; } set { _selectedTextColorBck = value; RefreshColors(); } }
        public Color UnselectedTextColor { get { return _unselectedTextColor; } set { _unselectedTextColor = value; RefreshColors(); } }
        public Color UnselectedTextColorBck { get { return _unselectedTextColorBck; } set { _unselectedTextColorBck = value; RefreshColors(); } }
        public Vector2 Position
        {
            get {return _position; }
            set
            {
                Vector2 difference = _position - value;
                foreach (Element e in Elements)
                {
                    e.Position += difference;
                }
                _position = value;
            }
        }
        #endregion

        #region Constructeur
        public GroupMenu()
        {
            _position = Vector2.Zero;
        }
        #endregion

        #region Méthodes

        #region Gestion des éléments
        public void AddElement(IIntegrableMenu pElement)
        {
            base.AddElement((Element)pElement);
            RefreshColors();
        }

        public void RemoveElement(IIntegrableMenu pElement)
        {
            base.RemoveElement((Element)pElement);
            CurrentSelection = CurrentSelection;
        }
        #endregion

        private void RefreshColors()
        {
            foreach (Element element in Elements)
            {
                IIntegrableMenu integrable = (IIntegrableMenu)element;
                integrable.Color_Selected = SelectedTextColor;
                integrable.ColorBck_Selected = SelectedTextColorBck;
                integrable.Color_Default = UnselectedTextColor;
                integrable.ColorBck_Default = UnselectedTextColorBck;
            }
        }

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Elements.Count > 0)
            {
                int i = 0;
                foreach (IIntegrableMenu e in Elements)
                {
                    e.Selected = (i == CurrentSelection);
                    i++;
                }
            }
        }
        #endregion

        #endregion
    }
}
