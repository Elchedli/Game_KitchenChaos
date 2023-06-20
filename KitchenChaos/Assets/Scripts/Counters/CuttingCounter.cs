using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter {

    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs {
        public float progressNormalized;
    }

    public event EventHandler OnCut;
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    
    private int cuttingProgress;
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // There is no kitchenObject here
            if (player.HasKitchenObject()) {
                //Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    // Player carrying something that can be cut.
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;
                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(GetKitchenObject().GetKitchenObjectSO());
                    OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs {
                        progressNormalized = (float) cuttingProgress / cuttingRecipeSO.cuttingProgressMax
                    });
                }
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
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
            //Their is a kitchen Object here
            cuttingProgress++;
            OnCut?.Invoke(this,EventArgs.Empty);

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(GetKitchenObject().GetKitchenObjectSO());
            OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });
            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }


    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(kitchenObjectSO);
        return cuttingRecipeSO != null;
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(inputKitchenObjectSO);
        if (cuttingRecipeSO != null) {
            return cuttingRecipeSO.output;
        }
        return null;
    }
    private CuttingRecipeSO GetCuttingRecipeSO(KitchenObjectSO inputKitchenObjectSO) {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray) {
            if (cuttingRecipeSO.input == inputKitchenObjectSO) {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
