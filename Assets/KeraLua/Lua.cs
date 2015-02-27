using System;
using System.Runtime.InteropServices;


namespace KeraLua
{
	public delegate int LuaNativeFunction (KeraLua.LuaState luaState);

	public static partial class Lua
	{
		public static int LuaGC (IntPtr luaState, int what, int data)
		{
			return NativeMethods.lua_gc (luaState, what, data);
		}

		public static CharPtr LuaTypeName (IntPtr luaState, int type)
		{
			return NativeMethods.lua_typename (luaState, type);
		}

		public static void LuaLError (IntPtr luaState, string message)
		{
			NativeMethods.luaL_error (luaState, message);
		}

		public static void LuaLWhere (IntPtr luaState, int level)
		{
			NativeMethods.luaL_where (luaState, level);
		}

		public static IntPtr LuaLNewState ()
		{
			return NativeMethods.luaL_newstate ();
		}

		public static void LuaClose (IntPtr luaState)
		{
			NativeMethods.lua_close (luaState);
		}

		public static void LuaLOpenLibs (IntPtr luaState)
		{
			NativeMethods.luaL_openlibs (luaState);
		}

		public static int LuaLLoadString (IntPtr luaState, string chunk)
		{
			return NativeMethods.luaL_loadstring (luaState, chunk);
		}

		public static int LuaLLoadString (IntPtr luaState, byte [] chunk)
		{
			return NativeMethods.luaL_loadstring (luaState, chunk.ToString());
		}

		public static void LuaCreateTable (IntPtr luaState, int narr, int nrec)
		{
			NativeMethods.lua_createtable (luaState, narr, nrec);
		}

		public static void LuaGetTable (IntPtr luaState, int index)
		{
			NativeMethods.lua_gettable (luaState, index);
		}

		public static void LuaSetTop (IntPtr luaState, int newTop)
		{
			NativeMethods.lua_settop (luaState, newTop);
		}

		public static void LuaInsert (IntPtr luaState, int newTop)
		{
			NativeMethods.lua_insert (luaState, newTop);
		}


		public static void LuaRemove (IntPtr luaState, int index)
		{
			NativeMethods.lua_remove (luaState, index);
		}


		public static void LuaRawGet (IntPtr luaState, int index)
		{
			NativeMethods.lua_rawget (luaState, index);
		}


		public static void LuaSetTable (IntPtr luaState, int index)
		{
			NativeMethods.lua_settable (luaState, index);
		}


		public static void LuaRawSet (IntPtr luaState, int index)
		{
			NativeMethods.lua_rawset (luaState, index);
		}


		public static void LuaSetMetatable (IntPtr luaState, int objIndex)
		{
			NativeMethods.lua_setmetatable (luaState, objIndex);
		}


		public static int LuaGetMetatable (IntPtr luaState, int objIndex)
		{
			return NativeMethods.lua_getmetatable (luaState, objIndex);
		}


		public static int LuaNetEqual (IntPtr luaState, int index1, int index2)
		{
			return NativeMethods.luanet_equal (luaState, index1, index2);
		}


		public static void LuaPushValue (IntPtr luaState, int index)
		{
			NativeMethods.lua_pushvalue (luaState, index);
		}


		public static void LuaReplace (IntPtr luaState, int index)
		{
			NativeMethods.lua_replace (luaState, index);
		}


		public static int LuaGetTop (IntPtr luaState)
		{
			return NativeMethods.lua_gettop (luaState);
		}


		public static int LuaType (IntPtr luaState, int index)
		{
			return NativeMethods.lua_type (luaState, index);
		}


		public static int LuaLRef (IntPtr luaState, int registryIndex)
		{
			return NativeMethods.luaL_ref (luaState, registryIndex);
		}

		public static void LuaRawGetI (IntPtr luaState, int tableIndex, int index)
		{
			NativeMethods.lua_rawgeti (luaState, tableIndex, index);
		}


		public static void LuaRawSetI (IntPtr luaState, int tableIndex, int index)
		{
			NativeMethods.lua_rawseti (luaState, tableIndex, index);
		}

		[CLSCompliantAttribute (false)]
		public static IntPtr LuaNewUserData (IntPtr luaState, uint size)
		{
			return NativeMethods.lua_newuserdata (luaState, size);
		}
		
		public static IntPtr LuaToUserData (IntPtr luaState, int index)
		{
			return NativeMethods.lua_touserdata (luaState, index);
		}


		public static void LuaLUnref (IntPtr luaState, int registryIndex, int reference)
		{
			NativeMethods.luaL_unref (luaState, registryIndex, reference);
		}


		public static int LuaIsString (IntPtr luaState, int index)
		{
			return NativeMethods.lua_isstring (luaState, index);
		}

		public static int LuaNetIsStringStrict (IntPtr luaState, int index)
		{
			return NativeMethods.luanet_isstring_strict (luaState, index);
		}


		public static bool LuaIsCFunction (IntPtr luaState, int index)
		{
			return NativeMethods.lua_iscfunction (luaState, index) != 0;
		}


		public static void LuaPushNil (IntPtr luaState)
		{
			NativeMethods.lua_pushnil (luaState);
		}


		public static int LuaNetPCall (IntPtr luaState, int nArgs, int nResults, int errfunc)
		{
			return NativeMethods.luanet_pcall (luaState, nArgs, nResults, errfunc);
		}

		public static LuaNativeFunction LuaToCFunction (IntPtr luaState, int index)
		{
			IntPtr ptr = NativeMethods.lua_tocfunction (luaState, index);
			if (ptr == IntPtr.Zero)
				return null;
#if NETFX_CORE
			LuaNativeFunction function = Marshal.GetDelegateForFunctionPointer <LuaNativeFunction> (ptr);
#else
			LuaNativeFunction function = Marshal.GetDelegateForFunctionPointer (ptr, typeof (LuaNativeFunction)) as LuaNativeFunction;
#endif
			return function;
		}


		public static double LuaNetToNumber (IntPtr luaState, int index)
		{
			return NativeMethods.luanet_tonumber (luaState, index);
		}


		public static int LuaToBoolean (IntPtr luaState, int index)
		{
			return NativeMethods.lua_toboolean (luaState, index);
		}

		[CLSCompliantAttribute (false)]
		public static CharPtr LuaToLString (IntPtr luaState, int index, out uint strLen)
		{
			return NativeMethods.lua_tolstring (luaState, index, out strLen);
		}

		public static void LuaAtPanic (IntPtr luaState, LuaNativeFunction panicf)
		{
			IntPtr fnpanic = Marshal.GetFunctionPointerForDelegate (panicf);
			NativeMethods.lua_atpanic (luaState, fnpanic);
		}

		public static void LuaPushStdCallCFunction (IntPtr luaState, LuaNativeFunction fn)
		{
			IntPtr pfunc = Marshal.GetFunctionPointerForDelegate (fn);
			NativeMethods.lua_pushstdcallcfunction (luaState, pfunc);
		}

		public static void LuaPushNumber (IntPtr luaState, double number)
		{
			NativeMethods.lua_pushnumber (luaState, number);
		}

		public static void LuaPushBoolean (IntPtr luaState, int value)
		{
			NativeMethods.lua_pushboolean (luaState, value);
		}

		[CLSCompliantAttribute (false)]
		public static void LuaNetPushLString (IntPtr luaState, string str, uint size)
		{
			NativeMethods.luanet_pushlstring (luaState, str, size);
		}

		public static void LuaPushString (IntPtr luaState, string str)
		{
			NativeMethods.lua_pushstring (luaState, str);
		}

		public static int LuaLNewMetatable (IntPtr luaState, string meta)
		{
			return NativeMethods.luaL_newmetatable (luaState, meta);
		}

		public static void LuaGetField (IntPtr luaState, int stackPos, string meta)
		{
			NativeMethods.lua_getfield (luaState, stackPos, meta);
		}

		public static IntPtr LuaLCheckUData (IntPtr luaState, int stackPos, string meta)
		{
			return NativeMethods.luaL_checkudata (luaState, stackPos, meta);
		}

		public static int LuaLGetMetafield (IntPtr luaState, int stackPos, string field)
		{
			return NativeMethods.luaL_getmetafield (luaState, stackPos, field);
		}

		[CLSCompliantAttribute (false)]
		public static int LuaNetLoadBuffer (IntPtr luaState, string buff, uint size, string name)
		{
            return NativeMethods.luanet_loadbuffer(luaState, buff, size, name);

		}

		[CLSCompliantAttribute (false)]
		public static int LuaNetLoadBuffer (IntPtr luaState, byte [] buff, uint size, string name)
		{
			return NativeMethods.luanet_loadbuffer (luaState, buff.ToString(), size, name);
		}

		public static int LuaNetLoadFile (IntPtr luaState, string filename)
		{
			return NativeMethods.luanet_loadfile (luaState, filename);
		}

		public static void LuaError (IntPtr luaState)
		{
			NativeMethods.lua_error (luaState);
		}

		public static int LuaCheckStack (IntPtr luaState, int extra)
		{
			return NativeMethods.lua_checkstack (luaState, extra);
		}

		public static int LuaNext (IntPtr luaState, int index)
		{
			return NativeMethods.lua_next (luaState, index);
		}

		public static void LuaPushLightUserData (IntPtr luaState, IntPtr udata)
		{
			NativeMethods.lua_pushlightuserdata (luaState, udata);
		}

		public static bool LuaLCheckMetatable (IntPtr luaState, int obj)
		{
			return NativeMethods.luaL_checkmetatable (luaState, obj) != 0;
		}

		public static int LuaGetHookMask (IntPtr luaState)
		{
			return NativeMethods.lua_gethookmask (luaState);
		}

		public static int LuaSetHook (IntPtr luaState, LuaHook func, int mask, int count)
		{
			IntPtr funcHook = func == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate (func);
			return NativeMethods.luanet_sethook(luaState, funcHook, mask, count);
		}

		public static int LuaGetHookCount (IntPtr luaState)
		{
            return NativeMethods.lua_gethookcount(luaState);
		}

		public static CharPtr LuaGetLocal (IntPtr luaState, LuaDebug ar, int n)
		{

			IntPtr pDebug = Marshal.AllocHGlobal (Marshal.SizeOf (ar));
			CharPtr local = IntPtr.Zero;

			try {
				Marshal.StructureToPtr (ar, pDebug, false);
                local = NativeMethods.lua_getlocal(luaState, pDebug, n);

			} finally {
				Marshal.FreeHGlobal (pDebug);
			}
			return local;
		}

		public static CharPtr LuaSetLocal (IntPtr luaState, LuaDebug ar, int n)
		{
			IntPtr pDebug = Marshal.AllocHGlobal (Marshal.SizeOf (ar));
			CharPtr local = IntPtr.Zero;

			try {
				Marshal.StructureToPtr (ar, pDebug, false);
                local = NativeMethods.lua_setlocal(luaState, pDebug, n);

			} finally {
				Marshal.FreeHGlobal (pDebug);
			}
			return local;
		}

		public static int LuaGetInfo (IntPtr luaState, string what,ref LuaDebug ar)
		{
			IntPtr pDebug = Marshal.AllocHGlobal (Marshal.SizeOf (ar));
			int ret = 0;

			try {
				Marshal.StructureToPtr (ar, pDebug, false);
                ret = NativeMethods.lua_getinfo(luaState, what, pDebug);
#if NETFX_CORE
				ar = Marshal.PtrToStructure <LuaDebug> (pDebug);
#else
				ar = (LuaDebug)Marshal.PtrToStructure (pDebug, typeof (LuaDebug));
#endif
			} finally {
				Marshal.FreeHGlobal (pDebug);
			}
			return ret;
		}

		public static int LuaGetStack (IntPtr luaState, int level,ref LuaDebug ar)
		{
			IntPtr pDebug = Marshal.AllocHGlobal (Marshal.SizeOf (ar));
			int ret = 0;
			try {
				Marshal.StructureToPtr (ar, pDebug, false);
                ret = NativeMethods.lua_getstack(luaState, level, pDebug);
#if NETFX_CORE
				ar = Marshal.PtrToStructure<LuaDebug> (pDebug);
#else
				ar = (LuaDebug)Marshal.PtrToStructure (pDebug, typeof (LuaDebug));
#endif
			} finally {
				Marshal.FreeHGlobal (pDebug);
			}
			return ret;
		}

  
		public static CharPtr LuaGetUpValue (IntPtr luaState, int funcindex, int n)
		{
            return NativeMethods.lua_getupvalue(luaState, funcindex, n);
		}

		public static CharPtr LuaSetUpValue (IntPtr luaState, int funcindex, int n)
		{
            return NativeMethods.lua_setupvalue(luaState, funcindex, n);
		}

		public static int LuaNetToNetObject (IntPtr luaState, int index)
		{
            return NativeMethods.luanet_tonetobject(luaState, index);
		}

		public static int LuaNetRegistryIndex ()
		{
            return NativeMethods.luanet_registryindex();
		}

		public static void LuaNetNewUData (IntPtr luaState, int val)
		{
            NativeMethods.luanet_newudata(luaState, val);
		}

		public static int LuaNetRawNetObj (IntPtr luaState, int obj)
		{
            return NativeMethods.luanet_rawnetobj(luaState, obj);
		}

		public static int LuaNetCheckUData (IntPtr luaState, int ud, string tname)
		{
            return NativeMethods.luanet_checkudata(luaState, ud, tname);
		}

		public static IntPtr LuaNetGetTag ()
		{
            return NativeMethods.luanet_gettag();
		}

		public static void LuaNetPushGlobalTable (IntPtr luaState)
		{
            NativeMethods.luanet_pushglobaltable(luaState);
		}

		public static void LuaNetPopGlobalTable (IntPtr luaState)
		{
			NativeMethods.luanet_popglobaltable (luaState);
		}

		public static void LuaNetSetGlobal (IntPtr luaState, string name)
		{
			NativeMethods.luanet_setglobal (luaState, name);
		}

		public static void LuaNetGetGlobal (IntPtr luaState, string name)
		{
			NativeMethods.luanet_getglobal (luaState, name);
		}

		public static IntPtr LuaNetGetMainState (IntPtr luaState)
		{
			return NativeMethods.luanet_get_main_state (luaState);
		}
	}

}

