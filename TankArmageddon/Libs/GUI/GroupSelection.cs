using Microsoft.Xna.Framework;
using System;

namespace TankArmageddon.GUI
{
    /// <summary>
    /// Interface permettant d'intégrer un élément à un menu de sélection (GroupMenu).
    /// </summary>
    public interface IIntegrableMenu
    {
        bool Selected { get; set; }
        Color Color_Selected { get; set;  }
        Color ColorBck_Selected { get; set; }
        Color Color_Default { get; set; }
        Color ColorBck_Default { get; set; }
    }

    /// <summary>
    /// Groupe d'éléments intégrés dans une sélection de type menu.
    /// </summary>
    public class GroupSelection : Group
    {
        #region Variables privées
        private int _currentSelection;
        private Color _selectedTextColor = Color.Yellow;
        private Color _selectedTextColorBck = Color.Red;
        private Color _unselectedTextColor = Color.Gray;
        private Color _unselectedTextColorBck = Color.Red;
        #endregion

        #region Propriétés
        public int CurrentSelection { get { return _currentSelection; } set { _currentSelection = (Elements.Count + value) % Elements.Count; ; } }
        public Color SelectedTextColor { get { return _selectedTextColor; } set { _selectedTextColor = value; RefreshColors(); } }
        public Color SelectedTextColorBck { get { return _selectedTextColorBck; } set { _selectedTextColorBck = value; RefreshColors(); } }
        public Color UnselectedTextColor { get { return _unselectedTextColor; } set { _unselectedTextColor = value; RefreshColors(); } }
        public Color UnselectedTextColorBck { get { return _unselectedTextColorBck; } set { _unselectedTextColorBck = value; RefreshColors(); } }
        #endregion

        #region Constructeur
        public GroupSelection() { }
        #endregion

        #region Méthodes

        #region Gestion des éléments
        public new void AddElement(Element pElement)
        {
            if (pElement is IIntegrableMenu)
            {
                base.AddElement(pElement);
            }
            else
            {
                throw new Exception("A GroupMenu can contain only IIntegrableMenu objects");
            }
        }

        public void AddElement(IIntegrableMenu pElement)
        {
            base.AddElement((Element)pElement);
            RefreshColors();
        }

        public void RemoveElement(IIntegrableMenu pElement)
        {
            base.RemoveElement((Element)pElement);
            // Non non, cette ligne n'est pas inutile ! 
            // Quand on supprime un élément, si la CurrentSelection pointait vers le dernier élément, la CurrentSelection doit être recalculée.
            // Voir dans le setter de CurrentSelection.
            CurrentSelection = CurrentSelection; 
        }
        #endregion

        #region Gestion des couleurs

        private void RefreshColors()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                IIntegrableMenu integrable = (IIntegrableMenu)Elements[i];
                integrable.Color_Selected = SelectedTextColor;
                integrable.ColorBck_Selected = SelectedTextColorBck;
                integrable.Color_Default = UnselectedTextColor;
                integrable.ColorBck_Default = UnselectedTextColorBck;
            }
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Elements.Count > 0)
            {
                for (int i = 0; i < Elements.Count; i++)
                {
                    IIntegrableMenu e = (IIntegrableMenu)Elements[i];
                    e.Selected = (i == CurrentSelection);
                }
            }
        }
        #endregion

        #endregion
    }
}
