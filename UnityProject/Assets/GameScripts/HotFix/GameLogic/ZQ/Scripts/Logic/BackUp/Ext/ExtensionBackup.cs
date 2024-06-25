
//#define DONT_USE_GENERATE_CODE                                                                 
//------------------------------------------------------------------------------                 
// <auto-generated>                                                                              
//     This code was generated by Lockstep.CodeGenerator                                         
//                                                                                               
//     Changes to this file may cause incorrect behavior and will be lost if                     
//     the code is regenerated.                                                                  
//     https://github.com/JiepengTan/LockstepPlatform                                            
// </auto-generated>                                                                             
//------------------------------------------------------------------------------                 
                                                                                                 
using Lockstep.Framework;                                                                                                                                      
using System.Text;                                                                          
                                                                   

namespace Lockstep.Game{                                                                                               
    public partial class CAnimator :IBackup{                                                                  
       public void WriteBackup(Serializer writer){                                           
			writer.Write(_animLen);
			writer.Write(_curAnimIdx);
			writer.Write(_curAnimName);
			writer.Write(_timer);
			writer.Write(configId);                                                                                     
       }                                                                                            
                                                                                                    
       public void ReadBackup(Deserializer reader){                                       
			_animLen = reader.ReadLFloat();
			_curAnimIdx = reader.ReadInt32();
			_curAnimName = reader.ReadString();
			_timer = reader.ReadLFloat();
			configId = reader.ReadInt32();                                                                                     
       }                                                                                            
                                                                                                    
       public int GetHash(ref int idx){                                      
           int hash = 1;                                                                             
			hash += _animLen.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += _curAnimIdx.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += _curAnimName.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += _timer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += configId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);                                                                                     
           return hash;                                                                                    
       }                                                                                            
                                                                                                    
       public void DumpStr(StringBuilder sb,string prefix){                                       
			sb.AppendLine(prefix + "_animLen"+":" + _animLen.ToString());
			sb.AppendLine(prefix + "_curAnimIdx"+":" + _curAnimIdx.ToString());
			sb.AppendLine(prefix + "_curAnimName"+":" + _curAnimName.ToString());
			sb.AppendLine(prefix + "_timer"+":" + _timer.ToString());
			sb.AppendLine(prefix + "configId"+":" + configId.ToString());                                                                                     
       }                                                                                            
    }                                                               
}                                                              

namespace Lockstep.Game{                                                                                               
    public partial class CBrain :IBackup{                                                                  
       public void WriteBackup(Serializer writer){                                           
			writer.Write(_atkTimer);
			writer.Write(atkInterval);
			writer.Write(stopDistSqr);
			writer.Write(targetId);                                                                                     
       }                                                                                            
                                                                                                    
       public void ReadBackup(Deserializer reader){                                       
			_atkTimer = reader.ReadLFloat();
			atkInterval = reader.ReadLFloat();
			stopDistSqr = reader.ReadLFloat();
			targetId = reader.ReadInt32();                                                                                     
       }                                                                                            
                                                                                                    
       public int GetHash(ref int idx){                                      
           int hash = 1;                                                                             
			hash += _atkTimer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += atkInterval.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += stopDistSqr.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += targetId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);                                                                                     
           return hash;                                                                                    
       }                                                                                            
                                                                                                    
       public void DumpStr(StringBuilder sb,string prefix){                                       
			sb.AppendLine(prefix + "_atkTimer"+":" + _atkTimer.ToString());
			sb.AppendLine(prefix + "atkInterval"+":" + atkInterval.ToString());
			sb.AppendLine(prefix + "stopDistSqr"+":" + stopDistSqr.ToString());
			sb.AppendLine(prefix + "targetId"+":" + targetId.ToString());                                                                                     
       }                                                                                            
    }                                                               
}                                                              

namespace Lockstep.Game{                                                                                               
    public partial class CMover :IBackup{                                                                  
       public void WriteBackup(Serializer writer){                                           
			writer.Write(hasReachTarget);
			writer.Write(needMove);                                                                                     
       }                                                                                            
                                                                                                    
       public void ReadBackup(Deserializer reader){                                       
			hasReachTarget = reader.ReadBoolean();
			needMove = reader.ReadBoolean();                                                                                     
       }                                                                                            
                                                                                                    
       public int GetHash(ref int idx){                                      
           int hash = 1;                                                                             
			hash += hasReachTarget.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += needMove.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);                                                                                     
           return hash;                                                                                    
       }                                                                                            
                                                                                                    
       public void DumpStr(StringBuilder sb,string prefix){                                       
			sb.AppendLine(prefix + "hasReachTarget"+":" + hasReachTarget.ToString());
			sb.AppendLine(prefix + "needMove"+":" + needMove.ToString());                                                                                     
       }                                                                                            
    }                                                               
}                                                              


namespace Lockstep.Game{                                                                                               
    public partial class CSkillBox :IBackup{                                                                  
       public void WriteBackup(Serializer writer){                                           
			writer.Write(_curSkillIdx);
			writer.Write(configId);
			writer.Write(isFiring);
			writer.Write(_skills);                                                                                     
       }                                                                                            
                                                                                                    
       public void ReadBackup(Deserializer reader){                                       
			_curSkillIdx = reader.ReadInt32();
			configId = reader.ReadInt32();
			isFiring = reader.ReadBoolean();
			_skills = reader.ReadList(this._skills);                                                                                     
       }                                                                                            
                                                                                                    
       public int GetHash(ref int idx){                                      
           int hash = 1;                                                                             
			//hash += _curSkillIdx.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			//hash += configId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			//hash += isFiring.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			//if(_skills != null) foreach (var item in _skills) {if(item != default(Lockstep.Game.Skill))hash += item.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);}                                                                                     
           return hash;                                                                                    
       }                                                                                            
                                                                                                    
       public void DumpStr(StringBuilder sb,string prefix){                                       
			sb.AppendLine(prefix + "_curSkillIdx"+":" + _curSkillIdx.ToString());
			sb.AppendLine(prefix + "configId"+":" + configId.ToString());
			sb.AppendLine(prefix + "isFiring"+":" + isFiring.ToString());
			BackUpUtil.DumpList("_skills", _skills, sb, prefix);                                                                                     
       }                                                                                            
    }                                                               
}                                                              

namespace Lockstep.Game{                                                                                               
    public partial class Enemy :IBackup{                                                                  
       public void WriteBackup(Serializer writer){                                           
			writer.Write(EntityId);
			writer.Write(PrefabId);
			writer.Write(curHealth);
			writer.Write(damage);
			writer.Write(isFire);
			writer.Write(isInvincible);
			writer.Write(maxHealth);
			writer.Write(moveSpd);
			writer.Write(turnSpd);
			animator.WriteBackup(writer);
			brain.WriteBackup(writer);
			colliderData.WriteBackup(writer);
			rigidbody.WriteBackup(writer);
			skillBox.WriteBackup(writer);
			transform.WriteBackup(writer);                                                                                     
       }                                                                                            
                                                                                                    
       public void ReadBackup(Deserializer reader){                                       
			EntityId = reader.ReadInt32();
			PrefabId = reader.ReadInt32();
			curHealth = reader.ReadInt32();
			damage = reader.ReadInt32();
			isFire = reader.ReadBoolean();
			isInvincible = reader.ReadBoolean();
			maxHealth = reader.ReadInt32();
			moveSpd = reader.ReadLFloat();
			turnSpd = reader.ReadLFloat();
			animator.ReadBackup(reader);
			brain.ReadBackup(reader);
			colliderData.ReadBackup(reader);
			rigidbody.ReadBackup(reader);
			skillBox.ReadBackup(reader);
			transform.ReadBackup(reader);                                                                                     
       }                                                                                            
                                                                                                    
       public int GetHash(ref int idx){                                      
           int hash = 1;                                                                             
			hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += PrefabId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += curHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += damage.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += isFire.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += isInvincible.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += maxHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += moveSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += turnSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += animator.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += brain.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += colliderData.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += rigidbody.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += skillBox.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += transform.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);                                                                                     
           return hash;                                                                                    
       }                                                                                            
                                                                                                    
       public void DumpStr(StringBuilder sb,string prefix){                                       
			sb.AppendLine(prefix + "EntityId"+":" + EntityId.ToString());
			sb.AppendLine(prefix + "PrefabId"+":" + PrefabId.ToString());
			sb.AppendLine(prefix + "curHealth"+":" + curHealth.ToString());
			sb.AppendLine(prefix + "damage"+":" + damage.ToString());
			sb.AppendLine(prefix + "isFire"+":" + isFire.ToString());
			sb.AppendLine(prefix + "isInvincible"+":" + isInvincible.ToString());
			sb.AppendLine(prefix + "maxHealth"+":" + maxHealth.ToString());
			sb.AppendLine(prefix + "moveSpd"+":" + moveSpd.ToString());
			sb.AppendLine(prefix + "turnSpd"+":" + turnSpd.ToString());
			sb.AppendLine(prefix + "animator" +":");  animator.DumpStr(sb,"\t" + prefix);
			sb.AppendLine(prefix + "brain" +":");  brain.DumpStr(sb,"\t" + prefix);
			sb.AppendLine(prefix + "colliderData" +":");  colliderData.DumpStr(sb,"\t" + prefix);
			sb.AppendLine(prefix + "rigidbody" +":");  rigidbody.DumpStr(sb,"\t" + prefix);
			sb.AppendLine(prefix + "skillBox" +":");  skillBox.DumpStr(sb,"\t" + prefix);
			sb.AppendLine(prefix + "transform" +":");  transform.DumpStr(sb,"\t" + prefix);                                                                                     
       }                                                                                            
    }                                                               
}                                                              

namespace Lockstep.Game{                                                                                               
    public partial class Player :IBackup{                                                                  
       public void WriteBackup(Serializer writer){                                           
			writer.Write(EntityId);
			writer.Write(PrefabId);
			writer.Write(curHealth);
			writer.Write(damage);
			writer.Write(isFire);
			writer.Write(isInvincible);
			writer.Write(localId);
			writer.Write(maxHealth);
			writer.Write(moveSpd);
			writer.Write(turnSpd);
			animator.WriteBackup(writer);
			colliderData.WriteBackup(writer);
			//input.WriteBackup(writer);
			mover.WriteBackup(writer);
			rigidbody.WriteBackup(writer);
			skillBox.WriteBackup(writer);
			transform.WriteBackup(writer);                                                                                     
       }                                                                                            
                                                                                                    
       public void ReadBackup(Deserializer reader){                                       
			EntityId = reader.ReadInt32();
			PrefabId = reader.ReadInt32();
			curHealth = reader.ReadInt32();
			damage = reader.ReadInt32();
			isFire = reader.ReadBoolean();
			isInvincible = reader.ReadBoolean();
			localId = reader.ReadInt32();
			maxHealth = reader.ReadInt32();
			moveSpd = reader.ReadLFloat();
			turnSpd = reader.ReadLFloat();
			animator.ReadBackup(reader);
			colliderData.ReadBackup(reader);
			//input.ReadBackup(reader);
			mover.ReadBackup(reader);
			rigidbody.ReadBackup(reader);
			skillBox.ReadBackup(reader);
			transform.ReadBackup(reader);                                                                                     
       }                                                                                            
                                                                                                    
       public int GetHash(ref int idx){                                      
           int hash = 1;                                                                             
			hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += PrefabId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += curHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += damage.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += isFire.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += isInvincible.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += localId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += maxHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += moveSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += turnSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += animator.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += colliderData.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			//hash += input.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += mover.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += rigidbody.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += skillBox.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += transform.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);                                                                                     
           return hash;                                                                                    
       }                                                                                            
                                                                                                    
       public void DumpStr(StringBuilder sb,string prefix){                                       
			sb.AppendLine(prefix + "EntityId"+":" + EntityId.ToString());
			sb.AppendLine(prefix + "PrefabId"+":" + PrefabId.ToString());
			sb.AppendLine(prefix + "curHealth"+":" + curHealth.ToString());
			sb.AppendLine(prefix + "damage"+":" + damage.ToString());
			sb.AppendLine(prefix + "isFire"+":" + isFire.ToString());
			sb.AppendLine(prefix + "isInvincible"+":" + isInvincible.ToString());
			sb.AppendLine(prefix + "localId"+":" + localId.ToString());
			sb.AppendLine(prefix + "maxHealth"+":" + maxHealth.ToString());
			sb.AppendLine(prefix + "moveSpd"+":" + moveSpd.ToString());
			sb.AppendLine(prefix + "turnSpd"+":" + turnSpd.ToString());
			sb.AppendLine(prefix + "animator" +":");  animator.DumpStr(sb,"\t" + prefix);
			sb.AppendLine(prefix + "colliderData" +":");  colliderData.DumpStr(sb,"\t" + prefix);
			//sb.AppendLine(prefix + "input" +":");  input.DumpStr(sb,"\t" + prefix);
			sb.AppendLine(prefix + "mover" +":");  mover.DumpStr(sb,"\t" + prefix);
			sb.AppendLine(prefix + "rigidbody" +":");  rigidbody.DumpStr(sb,"\t" + prefix);
			sb.AppendLine(prefix + "skillBox" +":");  skillBox.DumpStr(sb,"\t" + prefix);
			sb.AppendLine(prefix + "transform" +":");  transform.DumpStr(sb,"\t" + prefix);                                                                                     
       }                                                                                            
    }                                                               
}                                                              

namespace Lockstep.Game{                                                                                               
    public partial class Skill :IBackup{                                                                  
       public void WriteBackup(Serializer writer){                                           
			//writer.Write(CdTimer);
			//writer.Write(_curPartIdx);
			//writer.Write(skillTimer);
			//writer.Write((int)(State));
			//writer.Write(partCounter);                                                                                     
       }                                                                                            
                                                                                                    
       public void ReadBackup(Deserializer reader){                                       
			//CdTimer = reader.ReadLFloat();
			//_curPartIdx = reader.ReadInt32();
			//skillTimer = reader.ReadLFloat();
			//State = (ESkillState)reader.ReadInt32();
			//partCounter = reader.ReadArray(this.partCounter);                                                                                     
       }                                                                                            
                                                                                                    
       public int GetHash(ref int idx){                                      
           int hash = 1;                                                                             
			//hash += CdTimer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			//hash += _curPartIdx.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			//hash += skillTimer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			//hash += ((int)State) * PrimerLUT.GetPrimer(idx++);
			//if(partCounter != null) foreach (var item in partCounter) {if(item != default(System.Int32))hash += item.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);}                                                                                     
           return hash;                                                                                    
       }                                                                                            
                                                                                                    
       public void DumpStr(StringBuilder sb,string prefix){                                       
			//sb.AppendLine(prefix + "CdTimer"+":" + CdTimer.ToString());
			//sb.AppendLine(prefix + "_curPartIdx"+":" + _curPartIdx.ToString());
			//sb.AppendLine(prefix + "skillTimer"+":" + skillTimer.ToString());
			//sb.AppendLine(prefix + "State"+":" + State.ToString());
			//BackUpUtil.DumpList("partCounter", partCounter, sb, prefix);                                                                                     
       }                                                                                            
    }                                                               
}                                                              

namespace Lockstep.Game{                                                                                               
    public partial class Spawner :IBackup{                                                                  
       public void WriteBackup(Serializer writer){                                           
			writer.Write(EntityId);
			writer.Write(PrefabId);
			writer.Write(Timer);
			Info.WriteBackup(writer);
			transform.WriteBackup(writer);                                                                                     
       }                                                                                            
                                                                                                    
       public void ReadBackup(Deserializer reader){                                       
			EntityId = reader.ReadInt32();
			PrefabId = reader.ReadInt32();
			Timer = reader.ReadLFloat();
			Info.ReadBackup(reader);
			transform.ReadBackup(reader);                                                                                     
       }                                                                                            
                                                                                                    
       public int GetHash(ref int idx){                                      
           int hash = 1;                                                                             
			hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += PrefabId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += Timer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += Info.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += transform.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);                                                                                     
           return hash;                                                                                    
       }                                                                                            
                                                                                                    
       public void DumpStr(StringBuilder sb,string prefix){                                       
			sb.AppendLine(prefix + "EntityId"+":" + EntityId.ToString());
			sb.AppendLine(prefix + "PrefabId"+":" + PrefabId.ToString());
			sb.AppendLine(prefix + "Timer"+":" + Timer.ToString());
			sb.AppendLine(prefix + "Info" +":");  Info.DumpStr(sb,"\t" + prefix);
			sb.AppendLine(prefix + "transform" +":");  transform.DumpStr(sb,"\t" + prefix);                                                                                     
       }                                                                                            
    }                                                               
}

namespace Lockstep.Game
{
	public partial class SpawnerInfo : IBackup
	{
		public void WriteBackup(Serializer writer)
		{
			writer.Write(prefabId);
			writer.Write(spawnPoint);
			writer.Write(spawnTime);
		}

		public void ReadBackup(Deserializer reader)
		{
			prefabId = reader.ReadInt32();
			spawnPoint = reader.ReadLVector3();
			spawnTime = reader.ReadLFloat();
		}

		public int GetHash(ref int idx)
		{
			int hash = 1;
			hash += prefabId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += spawnPoint.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			hash += spawnTime.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
			return hash;
		}

		public void DumpStr(StringBuilder sb, string prefix)
		{
			sb.AppendLine(prefix + "prefabId" + ":" + prefabId.ToString());
			sb.AppendLine(prefix + "spawnPoint" + ":" + spawnPoint.ToString());
			sb.AppendLine(prefix + "spawnTime" + ":" + spawnTime.ToString());
		}
	}
}                                                             
