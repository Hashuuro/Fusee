/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 3.0.2
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace C4d {

public class ObjectColorProperties : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal ObjectColorProperties(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(ObjectColorProperties obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~ObjectColorProperties() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          C4dApiPINVOKE.delete_ObjectColorProperties(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public int usecolor {
    set {
      C4dApiPINVOKE.ObjectColorProperties_usecolor_set(swigCPtr, value);
    } 
    get {
      int ret = C4dApiPINVOKE.ObjectColorProperties_usecolor_get(swigCPtr);
      return ret;
    } 
  }

  public Fusee.Math.double3 /* Vector_cstype_out */ color {
    /* <Vector_csvarin> */
    set 
	{
      C4dApiPINVOKE.ObjectColorProperties_color_set(swigCPtr, value /* Vector_csin */);
    }  /* </Vector_csvarin> */   
   /* <Vector_csvarout> */
   get
   {  
      Fusee.Math.double3 ret = C4dApiPINVOKE.ObjectColorProperties_color_get(swigCPtr);
      return ret;
   } /* <Vector_csvarout> */ 
  }

  public bool xray {
    set {
      C4dApiPINVOKE.ObjectColorProperties_xray_set(swigCPtr, value);
    } 
    get {
      bool ret = C4dApiPINVOKE.ObjectColorProperties_xray_get(swigCPtr);
      return ret;
    } 
  }

  public ObjectColorProperties() : this(C4dApiPINVOKE.new_ObjectColorProperties(), true) {
  }

}

}