## Treasure Hunter


## 게임 개요

* 개발도구 : Unity 3D
* 플랫폼 : PC
* 시점 : 쿼터뷰
* 장르 : 3D RPG

## 게임 소개
도굴꾼이 된 검사가 몬스터를 사냥하며 금화와 경험치를 모아 성장하고 새로운 무기를 얻어 최종 보스 몬스터를 처치하는 것이 목적인 쿼터뷰 시점 3D RPG 게임

## 게임 시스템
* 레벨 시스템
* 스텟 시스템
* 스킬 시스템
* 인벤토리 시스템
* 상점 시스템

## 구현 특징
* Singleton
* Object Pooling
* Newtonsoft.Json 라이브러리 활용
* BehaviorTree 구현을 통한 Boss AI



# SingleTon

# BehavoirTree
![mermaid-diagram-2023-05-08-183754](https://github.com/cowkjw/Graduation/assets/83215829/13cbb036-3474-477a-9540-da655a2f122b)

<details>
<summary>Sequence Node</summary>
<div markdown="1">
```C#
public class Sequence : INode
{
    List<INode> children = new List<INode>();

    public void AddChild(INode child)
    {
        children.Add(child);
    }

    public bool Execute()
    {
        foreach (INode child in children)
        {
            if (!child.Execute())
            {
                return false;
            }
        }
        return true;
    }
}
````
</div>
</details>

# Object Pooling

