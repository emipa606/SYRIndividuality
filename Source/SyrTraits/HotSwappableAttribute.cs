using System;

namespace SyrTraits;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class HotSwappableAttribute : Attribute;