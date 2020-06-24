package org.firealarmsystem.ui.home

import android.graphics.Color
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.AdapterView
import android.widget.ArrayAdapter
import android.widget.ListView
import android.widget.Toast
import android.widget.Toast.LENGTH_LONG
import androidx.core.view.children
import androidx.fragment.app.Fragment
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProviders
import com.android.volley.Request
import com.android.volley.RequestQueue
import com.android.volley.Response
import com.android.volley.toolbox.JsonObjectRequest
import com.android.volley.toolbox.StringRequest
import com.android.volley.toolbox.Volley
import kotlinx.android.synthetic.main.fragment_home.*
import org.firealarmsystem.R
import org.json.JSONArray
import org.json.JSONObject
import java.util.*
import kotlin.concurrent.schedule


class HomeFragment : Fragment() {

    private lateinit var homeViewModel: HomeViewModel
    private lateinit var requestQueue : RequestQueue
    private val updateTimer = Timer()
    private lateinit var updateTimerTask : TimerTask
    private var selectedItemIndex : Int = -1

    override fun onCreateView(
            inflater: LayoutInflater,
            container: ViewGroup?,
            savedInstanceState: Bundle?
    ): View? {
        homeViewModel = activity?.run {
            ViewModelProviders.of(this).get(HomeViewModel::class.java)
        } ?: throw Exception("Invalid Activity")

        val root = inflater.inflate(R.layout.fragment_home, container, false)

        homeViewModel.title.observe(viewLifecycleOwner, Observer {
            text_home.text = it
        })

        val adapter: ArrayAdapter<String> = ArrayAdapter<String>(
            context!!,
            android.R.layout.simple_list_item_1
        )

        homeViewModel.gateways.observe(viewLifecycleOwner, Observer {
            adapter.clear()
            adapter.addAll(it)
        })
        val listViewGateway = root.findViewById<ListView>(R.id.listview_gateway)
        listViewGateway.adapter = adapter

        listViewGateway.onItemClickListener = AdapterView.OnItemClickListener(){
                adapterView: AdapterView<*> , view: View, index: Int, l: Long ->
            for(child in adapterView.children){
                child.setBackgroundColor(Color.WHITE)

                if(child == view){
                    selectedItemIndex = index
                    child.setBackgroundColor(Color.parseColor("#ffa4a2"))
                }
            }
        }

        homeViewModel.isAlarm.value = false
        /** Volley http request queue **/
        requestQueue = Volley.newRequestQueue(this.context)

        val obj = JSONObject()
        updateTimerTask  = updateTimer.schedule( 0 , 2000 ){
            val jsonRequest = StringRequest(Request.Method.POST, "https://1082-im.biyasu.com/api/CloudService/getGateWayList",
                Response.Listener { response ->
                    Log.i("response" , response)
                    val array = JSONArray(response.toString());
                    val tempList = ArrayList<String>()
                    var isAlarm = false
                    for(i in 0 until array.length()){
                        val item = array.getJSONObject(i);
                        if(item.getBoolean("isActive")){
                            tempList.add(item.getString("gateWayId"))

                            if(item.getBoolean("isAlarm")){
                                isAlarm = true
                            }
                        }
                    }

                    if(isAlarm){
                        if(!homeViewModel.isAlarm.value!!) {
                            homeViewModel.isAlarm.postValue(true)
                        }
                    }else {
                        homeViewModel.isAlarm.postValue(false)
                    }
                    homeViewModel.gateways.postValue(tempList)
                },
                Response.ErrorListener { error ->
                    Log.e("response" , error.toString())
                }
            )
            requestQueue.add(jsonRequest)
        }

        val btnAlarm = root.findViewById<AlarmButton>(R.id.btn_alarm)
        btnAlarm.setOnClickListener{
            if(homeViewModel.gateways.value != null && selectedItemIndex >= 0 && selectedItemIndex < homeViewModel.gateways.value!!.size){
                val jsonRequest = JsonObjectRequest(Request.Method.POST, "https://1082-im.biyasu.com/api/CloudService/userControlFire?onFireGateWayId=" + homeViewModel.gateways.value!!.elementAt(selectedItemIndex), null ,
                    Response.Listener { response ->
                        if(response.getBoolean("isAlarm")){
                            Toast.makeText(this.context , "成功" , LENGTH_LONG).show()
                        }else{
                            Toast.makeText(this.context , "失敗" , LENGTH_LONG).show()
                        }
                    },
                    Response.ErrorListener {
                        Toast.makeText(this.context , "網路錯誤" , LENGTH_LONG).show()
                    }
                )
                requestQueue.add(jsonRequest)
            }
        }

        return root
    }

    override fun onDestroy() {
        super.onDestroy()

        updateTimer.cancel()
        updateTimer.purge()
    }
}