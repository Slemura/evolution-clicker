using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.asosnovskiy.Components {

	public static class UnityObjectExtensions {

	    public static bool IsTrueNull(this UnityEngine.Object obj) {
	        /*if ((object)obj != null) {
                Debug.Log(obj.ToString());
	            return obj.ToString() == "null";
	        }
            */
	        return obj == null;
	    }


	    public static string ToMy(this UnityEngine.Object obj) {
	        return obj.ToString();
	    }
    }
}

