// Decompiled with JetBrains decompiler
// Type: Integro.InMeta.Runtime.ObjectViewThisObjectLoader
// Assembly: InMeta.ServerRuntime, Version=1.9.112.0, Culture=neutral, PublicKeyToken=null
// MVID: 0B76D0ED-50A9-498A-8B76-23FDD608972C
// Assembly location: V:\20210126 УЖКХ\InMeta_ServerRuntime_dll\InMeta.ServerRuntime.dll

namespace AploadPaymentsAccruals.Integro.InMeta.Runtime
{
  internal class ObjectViewThisObjectLoader : ObjectViewParameterLoader
  {
    public override void PrepareLoadPlan(LoadPlan plan, DataSession session)
    {
    }

    public override object GetValue(DataObject obj) => (object) new ReadOnlyDataObject(obj);
  }
}
