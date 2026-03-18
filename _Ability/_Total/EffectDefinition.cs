using UnityEngine;
//effect definition là lớp cơ sở cho tất cả các hiệu ứng chiêu,
// nó định nghĩa một phương thức Excute để thực hiện hiệu ứng dựa trên thông tin trong SkillContext và một phương thức Destroy để dọn dẹp sau khi hiệu ứng kết thúc. 
//Các hiệu ứng cụ thể như AoeEffect và DamageEffect sẽ kế thừa từ EffectDefinition và 
//triển khai các phương thức này để thực hiện hành động của mình khi chiêu được sử dụng.
public abstract class EffectDefinition : ScriptableObject
{

    public abstract void Excute(SkillContext ctx);
    public abstract void Destroy();
}
