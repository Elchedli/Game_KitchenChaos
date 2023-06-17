using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter {

    [SerializeField] private KitchenObjectSO cutKitchenObjectSO;
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // There is no kitchenObject here
            if (player.HasKitchenObject()) {
                //Player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            } else {
                //Player is not  carrying anything
            }
        } else {
            //There is a kitchenObject here
            if (player.HasKitchenObject()) {
                //Player is carrying something
            } else {
                // Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }

    }

    public override void InteractAlternate(Player player) {
        if (HasKitchenObject()) {
            //Their is a kitchen Object here
            GetKitchenObject().DestroySelf();
            Transform kitchenObjectTransform = Instantiate(cutKitchenObjectSO.prefab);
            kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
        }
    }
}
