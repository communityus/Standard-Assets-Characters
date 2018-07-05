﻿using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace StandardAssets.Characters.CharacterInput
{
	/// <summary>
	/// Simply polls on behalf of the LegacyUnityInputResponse
	/// </summary>
	public class LegacyUnityInputResponsePoller : MonoBehaviour
	{
		private static List<LegacyUnityInputResponsePoller> s_Pollers;
		
		/// <summary>
		/// The Input Response needed
		/// </summary>
		private LegacyUnityInputResponse response;
		
		/// <summary>
		/// Behaviour - hold/toggle
		/// </summary>
		private DefaultInputResponseBehaviour behaviour;
	
		/// <summary>
		/// Multi-purpose bool. For Toggles in represents the on state. For Holds it represents the previous button state
		/// </summary>
		private bool check;

		private string axisRaw;

		/// <summary>
		/// Called by the LegacyInputResponse
		/// </summary>
		/// <param name="newResponse"></param>
		/// <param name="newBehaviour"></param>
		/// <param name="axisRaw"></param>
		public void Init(LegacyUnityInputResponse newResponse, DefaultInputResponseBehaviour newBehaviour, String axisString)
		{
			response = newResponse;
			behaviour = newBehaviour;
			axisRaw = axisString;

			if (s_Pollers == null)
			{
				s_Pollers = new List<LegacyUnityInputResponsePoller>();
			}
			
			s_Pollers.Add(this);
		}
			
		/// <summary>
		/// Does polling of inputs
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private void Update()
		{
			switch (behaviour)
			{
				case DefaultInputResponseBehaviour.Hold:
					Hold();
					break;
				case DefaultInputResponseBehaviour.Toggle:
					Toggle();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		/// <summary>
		/// Logic for Holds
		/// </summary>
		private void Hold()
		{
			bool isAxis = Input.GetButton(axisRaw);
			
			if (!check && isAxis)
			{		
				OnStart();
			}

			if (check && !isAxis)
			{
				OnEnd();
			}
		
			check = isAxis;
		}

		/// <summary>
		/// Logic for Toggles
		/// </summary>
		private void Toggle()
		{
			if (Input.GetButtonDown(axisRaw))
			{
				if (!check)
				{
					OnStart();
				}
				else
				{
					OnEnd();
				}
				check = !check;
			}
		}

		private void OnStart()
		{
			if(s_Pollers!=null)
			{
				foreach (var poller in s_Pollers)
				{
					if (poller != this)
					{
						poller.check = false;
					}
				}
			}
			response.BroadcastStart();
		}

		private void OnEnd()
		{
			response.BroadcastEnd();
		}

		public void TouchScreenButtonToggle()
		{
			if (!check)
			{
				OnStart();
			}
			else
			{
				OnEnd();
			}
			check = !check;
		}
	}
}