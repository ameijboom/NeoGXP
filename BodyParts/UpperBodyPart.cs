using System;
using GXPEngine.GXPEngine.Core;

namespace GXPEngine.BodyParts;

public abstract class UpperBodyPart : BodyPart
{
    protected AnimationSprite? abilityModel;
    protected bool abilityUsable;
    protected int timeOfUsage;

    protected Vec2 abilityDirection;

    protected UpperBodyPart(string modelPath, int cols, int rows, int frames, Player player_) : base(modelPath, cols,
        rows, frames, player_)
    {
        abilityDirection = new Vec2(0, 0);
        abilityUsable = true;
    }

    protected override void Update()
    {
        abilityModel?.Animate(Time.deltaTime);

        if (abilityModel != null)
        {
            abilityDirection = new Vec2(Input.mouseX, Input.mouseY) - new Vec2(x + MyGame.playerBaseSize.x / 2, y + MyGame.partBaseSize.y);
            
            float newRotation = Vec2.WrapAroundDegree(abilityDirection.GetAngleDegrees());

            // if (newRotation is > 150 or < 30) abilityModel.rotation = newRotation;

            abilityModel.rotation = newRotation;
            
            
            if (Input.GetMouseButtonDown(0))
            {
                UseAbility();
                abilityUsable = false;
                timeOfUsage = Time.now;
            }
        }
    }

    public override void UpdatePosition()
    {
        SetXY(player.x, player.y);
    }

    protected void SetAbilityModel(string modelPath, int cols, int rows, int frames)
    {
        abilityModel = new AnimationSprite(modelPath, cols, rows, frames, true,false);
        abilityModel.SetXY(x + MyGame.playerBaseSize.x / 2, y + MyGame.partBaseSize.y/2);
        abilityModel.SetOrigin(MyGame.playerBaseSize.x/4, MyGame.partBaseSize.y/2);
        
        abilityDirection = new Vec2(Input.mouseX, Input.mouseY) - new Vec2(x + MyGame.playerBaseSize.x / 2, y + MyGame.partBaseSize.y);
        abilityModel.rotation = Vec2.WrapAroundDegree(abilityDirection.GetAngleDegrees());
        
        AddChild(abilityModel);


    }

    protected abstract void UseAbility();

}