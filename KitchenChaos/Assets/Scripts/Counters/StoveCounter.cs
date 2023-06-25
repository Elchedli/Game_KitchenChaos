using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter {

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs: EventArgs {
        public State state;
    }
    public enum State {
        Idle,
        Frying,
        Fried,
        Burned
    }
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private State state;
    private float fryingTimer;
    private FryingRecipeSO fryingRecipeSO;
    private float burningTimer;
    private BurningRecipeSO burningRecipeSO;

    private void Start() {
         state = State.Idle;
    }
    private void Update() {
        if (HasKitchenObject()) {
            switch (state) {
                case State.Idle:

                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;

                    if (fryingTimer > fryingRecipeSO.fryingTimerMax) {
                        //Fried
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                        state = State.Fried;
                        burningTimer = 0f;
                        burningRecipeSO = GetBurningRecipeSO(GetKitchenObject().GetKitchenObjectSO());
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = state
                        });
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    if (burningTimer > burningRecipeSO.burningTimerMax) {
                        //Fried
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);
                        state = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = state
                        });
                    }
                    break;
                case State.Burned:

                    break;

            }

        }

    }
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // There is no kitchenObject here
            if (player.HasKitchenObject()) {
                //Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    // Player carrying something that can be fried.
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingRecipeSO = GetFryingRecipeSO(GetKitchenObject().GetKitchenObjectSO());
                    state = State.Frying;
                    fryingTimer = 0f;
                   
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
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
                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                    state = state
                });
            }
        }

    }

    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO) {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSO(kitchenObjectSO);
        return fryingRecipeSO != null;
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSO(inputKitchenObjectSO);
        if (fryingRecipeSO != null) {
            return fryingRecipeSO.output;
        }
        return null;
    }
    private FryingRecipeSO GetFryingRecipeSO(KitchenObjectSO inputKitchenObjectSO) {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray) {
            if (fryingRecipeSO.input == inputKitchenObjectSO) {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSO(KitchenObjectSO inputKitchenObjectSO) {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray) {
            if (burningRecipeSO.input == inputKitchenObjectSO) {
                return burningRecipeSO;
            }
        }
        return null;
    }
}
