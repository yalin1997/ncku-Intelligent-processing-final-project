package org.firealarmsystem.ui.home

import android.graphics.Color
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.AdapterView
import android.widget.ArrayAdapter
import android.widget.ListView
import androidx.core.view.children
import androidx.fragment.app.Fragment
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProviders
import kotlinx.android.synthetic.main.fragment_home.*
import org.firealarmsystem.R


class HomeFragment : Fragment() {

    private lateinit var homeViewModel: HomeViewModel

    override fun onCreateView(
            inflater: LayoutInflater,
            container: ViewGroup?,
            savedInstanceState: Bundle?
    ): View? {
        homeViewModel =
                ViewModelProviders.of(this).get(HomeViewModel::class.java)
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
                    child.setBackgroundColor(Color.parseColor("#ffa4a2"))
                }
            }
        }

        homeViewModel.gateways.postValue(arrayListOf("Gateway1" , "Gateway2" , "Gateway3"))

        return root
    }
}
