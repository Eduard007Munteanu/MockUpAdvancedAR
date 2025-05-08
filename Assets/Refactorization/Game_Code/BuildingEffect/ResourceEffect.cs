// Create resource effect:
// Add 10 Food
// ResourceEffect effect = new ResourceEffect(ResourceType.Food, 10);
// When a pop is born add to food consumption
// ResourceEffect effect = new ResourceEffect(ResourceType.Food, 0, 0, 0, 0, -1);

// using System.Diagnostics;
using UnityEngine;

public class ResourceEffect{
    public ResourceType Type { get; }
    public float Amount { get; }
    public float Flat { get; }
    public float Mod1 { get; }
    public float Mod2 { get; }
    public float Constant { get; }
    ResourceDatabase resources = ResourceDatabase.Instance;
    public ResourceEffect(ResourceType type, float amount, float flat=0, float mod1=0, float mod2=0, float constant=0) {
        this.Type = type;
        this.Amount = amount;
        this.Flat = flat;
        this.Mod1 = mod1;
        this.Mod2 = mod2;
        this.Constant = constant;

        Debug.Log($"ResourceEffect created: {type}, Amount: {amount}, Flat: {flat}, Mod1: {mod1}, Mod2: {mod2}, Constant: {constant}");
    }
    public void Apply() {
        resources[Type].AddAmount(Amount);
        resources[Type].AddProductionModifier(Flat, Mod1, Mod2);
        resources[Type].AddProductionConstant(Constant);

        Debug.Log($"ResourceEffect applied: {Type}, Amount: {Amount}, Flat: {Flat}, Mod1: {Mod1}, Mod2: {Mod2}, Constant: {Constant}");
    }
    public void Cancel() {
        resources[Type].AddAmount(-Amount);
        resources[Type].AddProductionModifier(-Flat, -Mod1, -Mod2);
        resources[Type].AddProductionConstant(-Constant);

        Debug.Log($"ResourceEffect cancelled: {Type}, Amount: {-Amount}, Flat: {-Flat}, Mod1: {-Mod1}, Mod2: {-Mod2}, Constant: {-Constant}");
    }
}