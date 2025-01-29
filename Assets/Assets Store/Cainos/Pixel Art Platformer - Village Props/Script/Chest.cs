using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cainos.LucidEditor;

namespace Cainos.PixelArtPlatformer_VillageProps
{
    public class Chest : MonoBehaviour
    {
        [FoldoutGroup("Runtime"), Button("Open"), HorizontalGroup("Runtime/Button")]
        public void Open()
        {
            IsOpened = true;
        }

        [FoldoutGroup("Runtime"), Button("Close"), HorizontalGroup("Runtime/Button")]
        public void Close()
        {
            IsOpened = false;
        }

        [FoldoutGroup("Reference")]
        public Animator animator;

        [FoldoutGroup("Runtime"), ShowInInspector, DisableInEditMode]
        public bool IsOpened
        {
            get => isOpened;
            set
            {
                isOpened = value;
                animator.SetBool("IsOpened", isOpened);
            }
        }
        private bool isOpened;

        [SerializeField] private bool needKey;
        [SerializeField] private string keyId;
        [SerializeField] private int quantityKey = 1;
         private Inventory inventory;

        private void OnCollisionEnter(Collision other)
        {
            if (IsOpened) return;

            if (other.gameObject.CompareTag("Player"))
            {
                HeroKnight hero = other.gameObject.GetComponent<HeroKnight>();
                CanOpen(hero);
            }
        }

        private void CanOpen(HeroKnight hero)
        {
            if (needKey)
            {
                if (hero.inventory.HasKey(keyId, quantityKey))
                {
                    OpenChest(hero);
                }
                else
                {
                    Debug.Log("No tienes la llave o cantidad necesaria para abrir este cofre.");
                }
            }
            else
            {
                OpenChest(hero);
            }
        }

        private void OpenChest(HeroKnight hero)
        {
            IsOpened = true;
            hero.inventory.Add(inventory); // Agrega los ítems del cofre al inventario del jugador
            Debug.Log("Cofre abierto. Recompensas añadidas al inventario.");
        }
    }
}
