using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UniversalUnityHooks.AttributesHelper;
using static UniversalUnityHooks.Util;

namespace UniversalUnityHooks.Attributes
{
	public interface IAttribute
	{
		AddAttributesResponse AddAllFound();
		List<ReturnData<CustomAttribute>> AllAttributes { get; set; }
		List<ReturnData<CustomAttribute>> Attributes { get; set; }
		OperationTimer Timer { get; set; }
		int Count { get; }
	}
}
