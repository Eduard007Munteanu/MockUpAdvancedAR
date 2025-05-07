// Create resource effect:
// Add 10 Food
// ResourceEffect effect = new ResourceEffect(ResourceType.Food, 10);
// When a pop is born add to food consumption
// ResourceEffect effect = new ResourceEffect(ResourceType.Food, 0, 0, 0, 0, -1);

public class ResourceEffect{
    private ResourceType type;
    private float amount;
    private float flat;
    private float mod1;
    private float mod2;
    private float constant;
    ResourceDatabase resources = ResourceDatabase.Instance;
    public ResourceEffect(ResourceType type, float amount, float flat=0, float mod1=0, float mod2=0, float constant=0) {
        this.type = type;
        this.amount = amount;
        this.flat = flat;
        this.mod1 = mod1;
        this.mod2 = mod2;
        this.constant = constant;
    }
    public void Effect() {
        resources[type].AddAmount(amount);
        resources[type].AddProductionModifier(flat, mod1, mod2);
        resources[type].AddProductionConstant(constant);
    }
    public void NegativeEffect() {
        resources[type].AddAmount(-amount);
        resources[type].AddProductionModifier(-flat, -mod1, -mod2);
        resources[type].AddProductionConstant(-constant);
    }
}